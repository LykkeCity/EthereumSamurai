using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EthereumSamurai.Models.Blockchain
{
    public class TransactionModel
    {
        public BigInteger TransactionIndex { get; set; }
        public BigInteger BlockNumber { get; set; }
        public BigInteger Gas { get; set; }
        public BigInteger GasPrice { get; set; }
        public BigInteger Value { get; set; }
        public BigInteger Nonce { get; set; }
        public string TransactionHash { get; set; }
        public string BlockHash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Input { get; set; }
        public BigInteger BlockTimestamp { get; set; }
        public BigInteger GasUsed { get; set; }
        public string ContractAddress { get; set; }
        public bool HasError { get; set; }
    }

    public class TransactionFullInfoModel
    {
        public TransactionModel TransactionModel { get; set; }

        public IEnumerable<Erc20TransferHistoryModel> Erc20Transfers { get; set; }
    }
}
