using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Blockchain
{
    public class Erc20ContractModel
    {
        public string Address { get; set; }

        public string BlockHash { get; set; }

        public BigInteger BlockNumber { get; set; }

        public BigInteger BlockTimestamp { get; set; }

        public string DeployerAddress { get; set; }

        public uint? TokenDecimals { get; set; }

        public string TokenName { get; set; }

        public string TokenSymbol { get; set; }

        public BigInteger TokenTotalSupply { get; set; }

        public string TransactionHash { get; set; }
    }
}
