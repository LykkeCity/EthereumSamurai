using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Services
{
    public class Erc20BalanceService : IErc20BalanceService
    {
        private readonly IErc20BalanceRepository           _balanceRepository;
        private readonly IBlockIndexationHistoryRepository _blockIndexationHistoryRepository;
        private readonly IErc20TransferHistoryRepository   _transferHistoryRepository;



        public Erc20BalanceService(
            IErc20BalanceRepository           balanceRepository,
            IBlockIndexationHistoryRepository blockIndexationHistoryRepository,
            IErc20TransferHistoryRepository   transferHistoryRepository)
        {
            _balanceRepository                = balanceRepository;
            _blockIndexationHistoryRepository = blockIndexationHistoryRepository;
            _transferHistoryRepository        = transferHistoryRepository;
        }



        public async Task<IEnumerable<Erc20BalanceModel>> GetAsync(Erc20BalanceQuery query)
        {
            return await _balanceRepository.GetAsync(query);
        }

        public async Task<ulong?> GetNextBlockToIndexAsync(ulong startFrom)
        {
            return await _blockIndexationHistoryRepository.GetLowestBlockWithNotIndexedBalances(startFrom);
        }

        public async Task IndexBlockAsync(ulong blockNumber, int jobVersion)
        {
            var blockTransfers = await _transferHistoryRepository.GetAsync(new Erc20TransferHistoryQuery
            {
                BlockNumber = blockNumber
            });
            
            var deposits        = blockTransfers.Select(x => new { x.ContractAddress, AssetHolder = x.To,   TransferAmount = x.TransferAmount });
            var withdrawals     = blockTransfers.Select(x => new { x.ContractAddress, AssetHolder = x.From, TransferAmount = x.TransferAmount * -1 });
            var balanceChanges  = from transfer in deposits.Concat(withdrawals)
                                 group transfer
                                    by new { transfer.ContractAddress, transfer.AssetHolder }
                                  into g
                                select new
                                {
                                    AssetHolderAddress = g.Key.AssetHolder,
                                    ContractAddress    = g.Key.ContractAddress,
                                    BalanceChange      = g.Select(x => x.TransferAmount).Aggregate((a, b) => a + b).ToString(),
                                    BlockNumber        = blockNumber
                                };

            var newBalanceHistories = new List<Erc20BalanceModel>(balanceChanges.Count());

            foreach (var change in balanceChanges)
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
            }

            await _balanceRepository.SaveForBlockAsync(newBalanceHistories, blockNumber);
            await _blockIndexationHistoryRepository.MarkBalancesAsIndexed(blockNumber, jobVersion);
        }
    }
}