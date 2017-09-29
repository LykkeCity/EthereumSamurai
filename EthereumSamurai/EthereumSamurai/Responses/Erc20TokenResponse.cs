using System.Runtime.Serialization;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class Erc20TokenResponse
    {
        public string ContractAddress { get; set; }
        
        public uint? Decimals { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public string TotalSupply { get; set; }
    }
}