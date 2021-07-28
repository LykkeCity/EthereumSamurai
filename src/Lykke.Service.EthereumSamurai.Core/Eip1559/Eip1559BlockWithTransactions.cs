using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace Lykke.Service.EthereumSamurai.Core.Eip1559
{
    public class Eip1559BlockWithTransactions : Block
    {
        [JsonProperty(PropertyName = "baseFeePerGas")]
        public HexBigInteger BaseFeePerGas { get; set; }

        [JsonProperty(PropertyName = "transactions")]
        public Eip1559Transaction[] Transactions { get; set; }
    }
}
