using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.DebugModels
{
    public class TraceResultModel
    {
        public bool HasError { get; set; }

        public IEnumerable<TransferValueModel> Transfers { get; set; }
    }

    public enum TransferValueModelType
    {
        CREATION,
        TRANSACTION,
        TRANSFER
    }

    public class TransferValueModel
    {
        public int Depth { get; set; }
        public string TransactionHash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public BigInteger Value { get; set; }
        public TransferValueModelType Type { get; set; }
        public int MessageIndex { get; set; }
    }
}
