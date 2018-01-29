using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.DebugModels;
using Lykke.Service.EthereumSamurai.Services.Utils;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Filters;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class RpcBlockReader : IRpcBlockReader
    {
        private readonly IWeb3 _client;
        private readonly IDebug _debug;

        public RpcBlockReader(
            IWeb3 web3,
            IDebug debug)
        {
            _client = web3;
            _debug  = debug;
        }

        //just the tip
        public async Task<BigInteger> GetBlockCount()
        {
            var tip = await _client.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            return tip.Value;
        }

        public async Task<BlockContent> ReadBlockAsync(BigInteger blockHeight)
        {
            var block = await _client.Eth.Blocks
                .GetBlockWithTransactionsByNumber
                .SendRequestAsync(new HexBigInteger(blockHeight));

            var logs = new EthGetLogs(_client.Client);

            #region Block

            var blockHash  = block.BlockHash;
            var blockModel = new BlockModel
            {
                TransactionsCount = block.Transactions.Length,
                BlockHash         = blockHash,
                Difficulty        = block.Difficulty,
                ExtraData         = block.ExtraData,
                GasLimit          = block.GasLimit,
                GasUsed           = block.GasUsed,
                LogsBloom         = block.LogsBloom,
                Miner             = block.Miner,
                Nonce             = block.Nonce,
                Number            = block.Number,
                ParentHash        = block.ParentHash,
                ReceiptsRoot      = block.ReceiptsRoot,
                Sha3Uncles        = block.Sha3Uncles,
                Size              = block.Size,
                StateRoot         = block.StateRoot,
                Timestamp         = block.Timestamp,
                TotalDifficulty   = block.TotalDifficulty,
                TransactionsRoot  = block.TransactionsRoot
            };

            #endregion

            #region Transactions

            var internalMessages  = new List<InternalMessageModel>();
            var blockTransactions = new Dictionary<string, TransactionModel>(block.Transactions.Length);

            foreach (var transaction in block.Transactions)
            {
                var transactionReciept =
                    await _client.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction.TransactionHash);

                
                TraceResultModel traceResult = null;
                try
                {
                    traceResult = await _debug.TraceTransactionAsync
                    (
                        transaction.From,
                        transaction.To,
                        transactionReciept.ContractAddress,
                        transaction.Value.Value,
                        transaction.TransactionHash,
                        false,
                        true,
                        false
                    );

                    if (traceResult != null && !traceResult.HasError && traceResult.Transfers != null)
                    {
                        internalMessages.AddRange
                        (
                            traceResult.Transfers.Select(x => new InternalMessageModel
                            {
                                BlockNumber     = block.Number.Value,
                                Depth           = x.Depth,
                                FromAddress     = x.FromAddress,
                                MessageIndex    = x.MessageIndex,
                                ToAddress       = x.ToAddress,
                                TransactionHash = x.TransactionHash,
                                Value           = x.Value,
                                Type            = (InternalMessageModelType)x.Type,
                                BlockTimestamp  = blockModel.Timestamp
                            })
                        );
                    }

                }
                catch (Exception)
                {

                }

                var transactionModel = new TransactionModel
                {
                    BlockTimestamp   = block.Timestamp,
                    BlockHash        = transaction.BlockHash,
                    BlockNumber      = transaction.BlockNumber,
                    From             = transaction.From,
                    Gas              = transaction.Gas,
                    GasPrice         = transaction.GasPrice,
                    Input            = transaction.Input,
                    Nonce            = transaction.Nonce,
                    To               = transaction.To,
                    TransactionHash  = transaction.TransactionHash,
                    TransactionIndex = transaction.TransactionIndex,
                    Value            = transaction.Value,
                    GasUsed          = transactionReciept.GasUsed.Value,
                    ContractAddress  = transactionReciept.ContractAddress,
                    HasError         = traceResult?.HasError ?? false
                };

                blockTransactions[transaction.TransactionHash] = transactionModel;
            }

            var addressHistory = ExtractAddressHistory(internalMessages, blockTransactions.Values);

            #endregion

            #region Contracts

            var deployedContracts = new List<DeployedContractModel>();

            foreach (var transaction in blockTransactions.Select(x => x.Value).Where(x => x.ContractAddress != null))
            {
                deployedContracts.Add(new DeployedContractModel
                {
                    Address         = transaction.ContractAddress,
                    BlockHash       = blockHash,
                    BlockNumber     = block.Number.Value.ToString(),
                    BlockTimestamp  = block.Timestamp.Value.ToString(),
                    DeployerAddress = transaction.From,
                    TransactionHash = transaction.TransactionHash
                });
            }

            foreach (var message in internalMessages.Where(x => x.Type == InternalMessageModelType.CREATION))
            {
                deployedContracts.Add(new DeployedContractModel
                {
                    Address         = message.ToAddress,
                    BlockHash       = blockHash,
                    BlockNumber     = block.Number.Value.ToString(),
                    BlockTimestamp  = block.Timestamp.Value.ToString(),
                    DeployerAddress = message.FromAddress,
                    TransactionHash = message.TransactionHash
                });
            }

            // Select contracts with distinct addresses
            deployedContracts = deployedContracts.GroupBy(x => x.Address).Select(x => x.First()).ToList();

            #endregion

            #region Transfers

            var blockNumber = (ulong)blockHeight;
            var filter = new NewFilterInput
            {
                FromBlock = new BlockParameter(blockNumber),
                ToBlock = new BlockParameter(blockNumber),
                Topics = new object[]
                {
                    "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef"
                }
            };

            var transferLogs = await logs.SendRequestAsync(filter);
            var transfers = transferLogs
                .Where(x => x.Topics.Length == 3)
                .Select(x =>
                {
                    string trHash = x.TransactionHash;
                    TransactionModel transaction = null;
                    blockTransactions.TryGetValue(trHash, out transaction);

                    return new Erc20TransferHistoryModel
                    {
                        BlockHash = x.BlockHash,
                        BlockNumber = (ulong)x.BlockNumber.Value,
                        BlockTimestamp = (ulong)block.Timestamp.Value,
                        ContractAddress = x.Address,
                        From = x.GetAddressFromTopic(1),
                        LogIndex = (uint)x.LogIndex.Value,
                        To = x.GetAddressFromTopic(2),
                        TransactionHash = trHash,
                        TransactionIndex = (uint)x.TransactionIndex.Value,
                        TransferAmount = x.Data.HexToBigInteger(false),
                        GasUsed = transaction?.GasUsed ?? 0,
                        GasPrice = transaction?.GasPrice ?? 0
                    };
                })
                .ToList();

            #endregion

            return new BlockContent
            {
                AddressHistory    = addressHistory,
                BlockModel        = blockModel,
                DeployedContracts = deployedContracts,
                InternalMessages  = internalMessages,
                Transactions      = blockTransactions?.Select(x => x.Value).ToList(),
                Transfers         = transfers
            };
        }

        private static IEnumerable<AddressHistoryModel> ExtractAddressHistory(
            List<InternalMessageModel> internalMessages,
            IEnumerable<TransactionModel> blockTransactions)
        {
            var trHashIndexDictionary = new Dictionary<string, int>();
            var history = blockTransactions.Select(transaction =>
            {
                var index  = (int)transaction.TransactionIndex;
                var trHash = transaction.TransactionHash;

                trHashIndexDictionary[trHash] = index;

                return new AddressHistoryModel
                {
                    MessageIndex     = -1,
                    TransactionIndex = (int)transaction.TransactionIndex,
                    BlockNumber      = (ulong)transaction.BlockNumber,
                    BlockTimestamp   = (uint)transaction.BlockTimestamp,
                    From             = transaction.From,
                    HasError         = transaction.HasError,
                    To               = transaction.To,
                    TransactionHash  = transaction.TransactionHash,
                    Value            = transaction.Value,
                    GasUsed          = transaction.GasUsed,
                    GasPrice         = transaction.GasPrice
                };
            });

            history = history.Concat(internalMessages.Select(message => new AddressHistoryModel
            {
                MessageIndex     = message.MessageIndex,
                TransactionIndex = trHashIndexDictionary[message.TransactionHash],
                TransactionHash  = message.TransactionHash,
                To               = message.ToAddress,
                HasError         = false,
                From             = message.FromAddress,
                BlockNumber      = (ulong)message.BlockNumber,
                BlockTimestamp   = (uint)message.BlockTimestamp,
                Value            = message.Value,
                GasPrice         = 0,
                GasUsed          = 0
            }));

            return history;
        }
    }
}