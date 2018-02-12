using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Utils;
using Lykke.Service.EthereumSamurai.Models;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Roles.Interfaces
{
    public class BlockIndexingMiddleRole : IBlockIndexingRole
    {
        private readonly IBlockService _blockService;
        private readonly IIndexingService _indexingService;
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IErc20TransferHistoryService _erc20TransferHistoryService;

        public string Id => nameof(BlockIndexingMiddleRole);
        public int Version => 1;

        public BlockIndexingMiddleRole(IBlockService blockService,
            IIndexingService indexingService,
            IRpcBlockReader rpcBlockReader,
            IErc20TransferHistoryService erc20TransferHistoryService)
        {
            _blockService = blockService;
            _indexingService = indexingService;
            _rpcBlockReader = rpcBlockReader;
            _erc20TransferHistoryService = erc20TransferHistoryService;
        }

        public async Task<(BigInteger, IEnumerable<Erc20BalanceChangeCommand>)> IndexBlockAsync(BigInteger blockNumber)
        {
            BlockContent blockContent = null;
            int transactionCount = 0;
            ulong currentBlockNumber = (ulong)blockNumber;
            IEnumerable<Erc20BalanceChangeCommand> erc20Commands = null;

            await RetryPolicy.ExecuteAsync(async () =>
            {
                blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(blockNumber);

                var blockContext = new BlockContext(Id, Version, blockContent);
                var blockExists = (await _blockService.GetForHashAsync(blockContent.BlockModel.BlockHash)) != null;

                #region ERC_20 

                var currentTransfers = blockContext.BlockContent.Transfers;

                if (blockExists)
                {
                    //Calculate delta for previous and current transfers
                    var storedTransfers = await _erc20TransferHistoryService.GetAsync(new EthereumSamurai.Models.Query.Erc20TransferHistoryQuery()
                    {
                        BlockNumber = currentBlockNumber
                    });

                    erc20Commands = CalculateDelta(currentTransfers, storedTransfers, currentBlockNumber);
                }
                else
                {
                    erc20Commands = SelectBlockBalanceChangeEvents(currentTransfers, currentBlockNumber)?.Values;
                }

                #endregion

                await _indexingService.IndexBlockAsync(blockContext);

            }, 5, 100);

            var nextBlockTooIndex = blockNumber + 1;

            return (nextBlockTooIndex, erc20Commands);
        }

        private IEnumerable<Erc20BalanceChangeCommand> CalculateDelta(IEnumerable<Erc20TransferHistoryModel> newTransfers, 
            IEnumerable<Erc20TransferHistoryModel> oldTransfers, ulong blockNumber)
        {
            var newEvents = SelectBlockBalanceChangeEvents(newTransfers, blockNumber);
            var oldEvents = SelectBlockBalanceChangeEvents(oldTransfers, blockNumber, - 1);

            foreach (var item in oldEvents)
            {
                Erc20BalanceChangeCommand newCommand = null;
                newEvents.TryGetValue(item.Key, out newCommand);
                if (newCommand == null)
                {
                    newEvents[item.Key] = item.Value;
                }
                else
                {
                    newCommand.BalanceChange += item.Value.BalanceChange;
                }
            }

            return newEvents.Values;
        }

        private Dictionary<(string holder, string contract), Erc20BalanceChangeCommand> SelectBlockBalanceChangeEvents(
            IEnumerable<Erc20TransferHistoryModel> blockTransfers, ulong blockNumber, int multiplier = 1)
        {
            var deposits = blockTransfers.Select(x => new { x.ContractAddress, AssetHolder = x.To, TransferAmount = x.TransferAmount * multiplier});
            var withdrawals = blockTransfers.Select(x => new { x.ContractAddress, AssetHolder = x.From, TransferAmount = x.TransferAmount * (-multiplier) });
            var balanceChanges = (from transfer in deposits.Concat(withdrawals)
                                  group transfer
                                    by new { transfer.ContractAddress, transfer.AssetHolder }
                                  into g
                                  select new Erc20BalanceChangeCommand()
                                  {
                                      AssetHolderAddress = g.Key.AssetHolder,
                                      ContractAddress = g.Key.ContractAddress,
                                      BalanceChange = g.Select(x => x.TransferAmount).Aggregate((a, b) => a + b),
                                      BlockNumber = blockNumber
                                  }).ToDictionary(x => (x.AssetHolderAddress, x.ContractAddress));

            return balanceChanges;
        }

        /*
         public async Task<BigInteger> IndexBlockAsync(BigInteger blockNumber)
        {
            var currentBlockNumber = blockNumber;
            int iterationVector = 0;
            BlockContent blockContent = null;
            int transactionCount = 0;

            await RetryPolicy.ExecuteAsync(async () =>
            {
                blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);

                var blockContext = new BlockContext(Id, Version, blockContent);
                var blockExists = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);

                transactionCount = blockContent.Transactions.Count;
                iterationVector = blockExists ? 1 : -1; //That is how we deal with forks

                await _indexingService.IndexBlockAsync(blockContext);

            }, 5, 100);

            var nextBlockTooIndex = currentBlockNumber + iterationVector;

            return nextBlockTooIndex;
        }
         */
    }
}
