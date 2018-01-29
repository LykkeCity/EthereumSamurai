using EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using EthereumSamurai.Models;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;

namespace EthereumSamurai.UnitTest.Mocks
{
    public class MockRpcBlockReader : IRpcBlockReader
    {
        private List<BlockContent> _blocks = new List<BlockContent>()
        {
            new BlockContent()
            {
                BlockModel = new Models.Blockchain.BlockModel()
                {
                    BlockHash = "testHash",
                    Difficulty = new BigInteger(30000),
                    ExtraData = "none",
                    GasLimit= new BigInteger(30000),
                    GasUsed= new BigInteger(15000),
                    LogsBloom = "",
                    Miner= "testAddress",
                    Nonce = "1",
                    Number = new BigInteger(1),
                    ParentHash = null,
                    ReceiptsRoot= "",
                    Sha3Uncles= "",
                    Size= new BigInteger(30000),
                    StateRoot = "",
                    Timestamp= 1000,
                    TotalDifficulty= new BigInteger(30000),
                    TransactionsCount= 1,
                    TransactionsRoot = ""
                },
                Transactions = new List<Models.Blockchain.TransactionModel>()
                {
                    new Models.Blockchain.TransactionModel()
                    {
                        BlockHash = "testHash",
                        BlockNumber = 1,
                        BlockTimestamp = 1000,
                        From = "testAddress",
                        Gas = new BigInteger(30000),
                        GasPrice = new BigInteger(0),
                        Input = "testData",
                        Nonce = new BigInteger(0),
                        To = "anotherAddress",
                        TransactionHash = "testTransactionHast",
                        TransactionIndex = new BigInteger(1),
                        Value = new BigInteger(100000000000000000)
                    }
                },
            }
        };

        public Task<BigInteger> GetBlockCount()
        {
            return Task.FromResult(new BigInteger(_blocks.Count));
        }

        public Task<BlockContent> ReadBlockAsync(BigInteger blockHeight)
        {
            BlockContent blockContent = _blocks.FirstOrDefault(x => x.BlockModel.Number == blockHeight);
            BlockContent copy = Force.DeepCloner.DeepClonerExtensions.DeepClone(blockContent);

            return Task.FromResult(copy);
        }
    }
}
