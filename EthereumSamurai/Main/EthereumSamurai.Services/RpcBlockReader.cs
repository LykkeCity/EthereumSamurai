using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.DebugModels;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class RpcBlockReader : IRpcBlockReader
    {
        private readonly IBaseSettings _bitcoinIndexerSettings;
        private readonly IWeb3 _client;
        private readonly IDebug _debug;

        public RpcBlockReader(IBaseSettings bitcoinIndexerSettings, IWeb3 web3, IDebug debug)
        {
            _bitcoinIndexerSettings = bitcoinIndexerSettings;
            _client = web3;
            _debug = debug;
        }

        public async Task<BlockContent> ReadBlockAsync(BigInteger blockHeight)
        {
            BlockWithTransactions block = await _client.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockHeight));

            #region Block

            string blockHash = block.BlockHash;
            BlockModel blockModel = new BlockModel()
            {
                TransactionsCount = block.Transactions.Length,
                BlockHash = block.BlockHash,
                Difficulty = block.Difficulty,
                ExtraData = block.ExtraData,
                GasLimit = block.GasLimit,
                GasUsed = block.GasUsed,
                LogsBloom = block.LogsBloom,
                Miner = block.Miner,
                Nonce = block.Nonce,
                Number = block.Number,
                ParentHash = block.ParentHash,
                ReceiptsRoot = block.ReceiptsRoot,
                Sha3Uncles = block.Sha3Uncles,
                Size = block.Size,
                StateRoot = block.StateRoot,
                Timestamp = block.Timestamp,
                TotalDifficulty = block.TotalDifficulty,
                TransactionsRoot = block.TransactionsRoot,

            };

            #endregion

            #region Transactions

            List<InternalMessageModel> internalMessages = new List<InternalMessageModel>();
            List<TransactionModel> blockTransactions = new List<TransactionModel>(block.Transactions.Length);

            foreach (var transaction in block.Transactions)
            {
                TransactionReceipt transactionReciept = await _client.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction.TransactionHash);

                TraceResultModel traceResult = null;
                try
                {
                    traceResult = await _debug.TraceTransactionAsync(transaction.From,
                    transaction.To,
                    transactionReciept.ContractAddress,
                    transaction.Value.Value,
                    transaction.TransactionHash,
                    withMemory: false,
                    withStack: true,
                    withStorage: false);

                    if (traceResult?.Transfers != null)
                    {
                        internalMessages.AddRange(traceResult.Transfers.Select(x => new InternalMessageModel()
                        {
                            BlockNumber = block.Number.Value,
                            Depth = x.Depth,
                            FromAddress = x.FromAddress,
                            MessageIndex = x.MessageIndex,
                            ToAddress = x.ToAddress,
                            TransactionHash = x.TransactionHash,
                            Value = x.Value,
                            Type = (InternalMessageModelType)x.Type
                        }));
                    }
                }
                catch (Exception e)
                { }

                TransactionModel transactionModel = new TransactionModel()
                {
                    BlockTimestamp = block.Timestamp,
                    BlockHash = transaction.BlockHash,
                    BlockNumber = transaction.BlockNumber,
                    From = transaction.From,
                    Gas = transaction.Gas,
                    GasPrice = transaction.GasPrice,
                    Input = transaction.Input,
                    Nonce = transaction.Nonce,
                    To = transaction.To,
                    TransactionHash = transaction.TransactionHash,
                    TransactionIndex = transaction.TransactionIndex,
                    Value = transaction.Value,
                    GasUsed = transactionReciept.GasUsed.Value,
                    ContractAddress = transactionReciept.ContractAddress,
                    HasError = traceResult?.HasError ?? false
                };

                blockTransactions.Add(transactionModel);
            }

            #endregion

            return new BlockContent()
            {
                InternalMessages = internalMessages,
                Transactions = blockTransactions,
                BlockModel = blockModel
            };
        }

        //just the tip
        public async Task<BigInteger> GetBlockCount()
        {
            HexBigInteger tip = await _client.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            return tip.Value;
        }
    }
}
