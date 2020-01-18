using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Common.Log;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class Erc20BalanceIndexingService : IErc20BalanceIndexingService
    {
        private readonly ILog _log;
        private readonly IErc20BalanceRepository           _balanceRepository;
        private readonly IBlockIndexationHistoryRepository _blockIndexationHistoryRepository;
        private readonly IErc20TransferHistoryRepository   _transferHistoryRepository;
        private readonly IIndexingRabbitNotifier           _indexingRabbitNotifier;


        public Erc20BalanceIndexingService(
            ILog log, 
            IErc20BalanceRepository           balanceRepository,
            IBlockIndexationHistoryRepository blockIndexationHistoryRepository,
            IErc20TransferHistoryRepository   transferHistoryRepository,
            IIndexingRabbitNotifier           indexingRabbitNotifier)
        {
            _log = log.CreateComponentScope(nameof(Erc20BalanceIndexingService));
            _balanceRepository                = balanceRepository;
            _blockIndexationHistoryRepository = blockIndexationHistoryRepository;
            _transferHistoryRepository        = transferHistoryRepository;
            _indexingRabbitNotifier           = indexingRabbitNotifier;
        }
        

        public async Task<ulong?> GetNextBlockToIndexAsync(ulong startFrom)
        {
            return await _blockIndexationHistoryRepository.GetLowestBlockWithNotIndexedBalances(startFrom);
        }

        public async Task IndexBlockAsync(ulong blockNumber, int jobVersion)
        {
            var blockTransfers = (await _transferHistoryRepository.GetAsync(new Erc20TransferHistoryQuery
            {
                BlockNumber = blockNumber
            })).ToArray();

            _log.WriteInfo(
                nameof(IndexBlockAsync),
                new
                {
                    blockNumber = blockNumber,
                    blockTransfersCount = blockTransfers.Length
                },
                "blockTransfers read");
            
            var deposits       = blockTransfers.Select(x => new { x.ContractAddress, AssetHolder = x.To,   TransferAmount = x.TransferAmount });
            var withdrawals    = blockTransfers.Select(x => new { x.ContractAddress, AssetHolder = x.From, TransferAmount = x.TransferAmount * -1 });
            var balanceChanges = (from transfer in deposits.Concat(withdrawals)
                                 group transfer
                                    by new { transfer.ContractAddress, transfer.AssetHolder }
                                  into g
                                 select new
                                 {
                                     AssetHolderAddress = g.Key.AssetHolder,
                                     ContractAddress    = g.Key.ContractAddress,
                                     BalanceChange      = g.Select(x => x.TransferAmount).Aggregate((a, b) => a + b).ToString(),
                                     BlockNumber        = blockNumber
                                 }).ToArray();

            _log.WriteInfo(
                nameof(IndexBlockAsync),
                new
                {
                    blockNumber = blockNumber,
                    balanceChangesCount = balanceChanges.Length
                },
                "balanceChanges evaluated");

            var newBalanceHistories = new ConcurrentBag<Erc20BalanceModel>();
            
            await Task.WhenAll(balanceChanges.Select(async change =>
            {
                var balanceHistory = await _balanceRepository.GetPreviousAsync(change.AssetHolderAddress, change.ContractAddress, change.BlockNumber);
                var balanceChange  = BigInteger.Parse(change.BalanceChange);

                if (balanceHistory != null)
                {
                    balanceHistory.Balance += balanceChange;
                }
                else
                {
                    balanceHistory = new Erc20BalanceModel
                    {
                        AssetHolderAddress = change.AssetHolderAddress,
                        Balance            = balanceChange,
                        ContractAddress    = change.ContractAddress
                    };
                }

                newBalanceHistories.Add(balanceHistory);
            }));

            _log.WriteInfo(
                nameof(IndexBlockAsync),
                new
                {
                    blockNumber = blockNumber,
                    newBalanceHistoriesCount = newBalanceHistories.Count
                },
                "newBalanceHistories evaluated");
            
            await _balanceRepository.SaveForBlockAsync(newBalanceHistories, blockNumber);

            _log.WriteInfo(
                nameof(IndexBlockAsync),
                new
                {
                    blockNumber = blockNumber
                },
                "newBalanceHistories saved");
            
            await _indexingRabbitNotifier.NotifyAsync(new Lykke.Service.EthereumSamurai.Models.Messages.RabbitIndexingMessage()
            {
                BlockNumber         = blockNumber,
                IndexingMessageType = EthereumSamurai.Models.Messages.IndexingMessageType.ErcBalances
            });

            _log.WriteInfo(
                nameof(IndexBlockAsync),
                new
                {
                    blockNumber = blockNumber
                },
                "RabbitIndexingMessage sent");
            
            await _blockIndexationHistoryRepository.MarkBalancesAsIndexed(blockNumber, jobVersion);

            _log.WriteInfo(
                nameof(IndexBlockAsync),
                new
                {
                    blockNumber = blockNumber
                },
                "balances marked as indexed");
        }
    }
}