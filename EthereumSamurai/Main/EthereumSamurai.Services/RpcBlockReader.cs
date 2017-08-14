using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Services.Erc20;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.DebugModels;
using EthereumSamurai.Services.Utils;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Filters;

namespace EthereumSamurai.Services
{
    public class RpcBlockReader : IRpcBlockReader
    {
        private readonly IBaseSettings  _bitcoinIndexerSettings;
        private readonly IWeb3          _client;
        private readonly IDebug         _debug;
        private readonly IErc20Detector _erc20Detector;

        public RpcBlockReader(IBaseSettings bitcoinIndexerSettings, IWeb3 web3, IErc20Detector erc20Detector, IDebug debug)
        {
            _bitcoinIndexerSettings = bitcoinIndexerSettings;
            _client                 = web3;
            _debug                  = debug;
            _erc20Detector          = erc20Detector;
        }

        //just the tip
        public async Task<BigInteger> GetBlockCount()
        {
            var tip = await _client.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            return tip.Value;
        }

        public async Task<BlockContent> ReadBlockAsync(BigInteger blockHeight)
        {
            var block = await _client.Eth.Blocks.GetBlockWithTransactionsByNumber
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
            var blockTransactions = new List<TransactionModel>(block.Transactions.Length);

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

                blockTransactions.Add(transactionModel);
            }

            var addressHistory = ExtractAddressHistory(internalMessages, blockTransactions);

            #endregion

            #region Contracts

            var contractInfoMap   = new Dictionary<string, Tuple<string, string>>();
            var deployedContracts = blockTransactions
                .Where(x => x.ContractAddress != null)
                .Select(x =>
                    {
                        var contractAddress = x.ContractAddress;

                        contractInfoMap[contractAddress] = Tuple.Create(x.TransactionHash, x.From);

                        return contractAddress;
                    })
                .Concat(internalMessages.Where(x => x.Type == InternalMessageModelType.CREATION).Select(x =>
                    {
                        var contractAddress = x.ToAddress;

                        contractInfoMap[contractAddress] = Tuple.Create(x.TransactionHash, x.FromAddress);

                        return contractAddress;
                    }));

            var erc20Contracts = new List<Erc20ContractModel>();

            foreach (var contractAddress in deployedContracts)
            {
                var isCompatible = await _erc20Detector.IsContractErc20Compatible(contractAddress);
                if (isCompatible)
                {
                    var tuple = contractInfoMap[contractAddress];

                    erc20Contracts.Add(new Erc20ContractModel
                    {
                        Address         = contractAddress,
                        BlockHash       = blockHash,
                        BlockNumber     = block.Number.Value,
                        BlockTimestamp  = block.Timestamp.Value,
                        DeployerAddress = tuple.Item2,
                        TokenName       = "",
                        TransactionHash = tuple.Item1
                    });
                }
            }

            #endregion

            #region Transfers
            
            var blockNumber     = (ulong) blockHeight;
            var filter          = new NewFilterInput
            {
                FromBlock = new BlockParameter(blockNumber),
                ToBlock   = new BlockParameter(blockNumber),
                Topics    = new object[]
                {
                    "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef"
                }
            };
            
            var transferLogs = await logs.SendRequestAsync(filter);
            var transfers    = transferLogs.Select(x => new Erc20TransferHistoryModel
            {
                BlockHash         = x.BlockHash,
                BlockNumber       = (ulong) x.BlockNumber.Value,
                BlockTimestamp    = (ulong) block.Timestamp.Value,
                ContractAddress   = x.Address,
                From              = x.Topics[1].ToString().TrimLeadingZeroes(),
                LogIndex          = (uint) x.LogIndex.Value,
                To                = x.Topics[2].ToString().TrimLeadingZeroes(),
                TransactionHash   = x.TransactionHash,
                TransactionIndex  = (uint) x.TransactionIndex.Value,
                TransferAmount    = x.Data.HexToBigInteger(false)
            }).ToList();

            #endregion

            return new BlockContent
            {
                AddressHistory        = addressHistory,
                BlockModel            = blockModel,
                CreatedErc20Contracts = erc20Contracts,
                InternalMessages      = internalMessages,
                Transactions          = blockTransactions,
                Transfers             = transfers
            };
        }

        private static IEnumerable<AddressHistoryModel> ExtractAddressHistory(List<InternalMessageModel> internalMessages, List<TransactionModel> blockTransactions)
        {
            var trHashIndexDictionary = new Dictionary<string, int>();
            var history               = blockTransactions.Select(transaction =>
            {
                var index  = (int) transaction.TransactionIndex;
                var trHash = transaction.TransactionHash;

                trHashIndexDictionary[trHash] = index;

                return new AddressHistoryModel
                {
                    MessageIndex     = -1,
                    TransactionIndex = (int) transaction.TransactionIndex,
                    BlockNumber      = (ulong) transaction.BlockNumber,
                    BlockTimestamp   = (uint) transaction.BlockTimestamp,
                    From             = transaction.From,
                    HasError         = transaction.HasError,
                    To               = transaction.To,
                    TransactionHash  = transaction.TransactionHash,
                    Value            = transaction.Value
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
                BlockNumber      = (ulong) message.BlockNumber,
                BlockTimestamp   = (uint) message.BlockTimestamp,
                Value            = message.Value
            }));

            return history;
        }
    }
}