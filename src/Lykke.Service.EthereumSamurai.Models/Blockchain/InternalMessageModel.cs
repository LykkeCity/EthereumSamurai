using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Blockchain
{
    public enum InternalMessageModelType
    {
        CREATION,
        TRANSACTION,
        TRANSFER
    }

    public class InternalMessageModel
    {
        public string TransactionHash { get; set; }
        public BigInteger BlockNumber { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public int Depth { get; set;}
        public BigInteger Value { get; set; }
        public int MessageIndex { get; set; }
        public InternalMessageModelType Type { get; set; }
        public BigInteger BlockTimestamp { get; set; }
    }
}
