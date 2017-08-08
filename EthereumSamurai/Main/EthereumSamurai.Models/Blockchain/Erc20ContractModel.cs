﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EthereumSamurai.Models.Blockchain
{
    public class Erc20ContractModel
    {
        public string Address { get; set; }

        public string TokenName { get; set; }

        public BigInteger BlockNumber { get; set; }

        public string BlockHash { get; set; }

        public string TransactionHash { get; set; }

        public string DeployerAddress { get; set; }

        public BigInteger BlockTimestamp { get; set; }
    }
}
