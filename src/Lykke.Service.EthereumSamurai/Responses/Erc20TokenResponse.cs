using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Responses
{
    [DataContract]
    public class Erc20TokenResponse
    {
        [DataMember(Name = "address")]
        public string ContractAddress { get; set; }

        [DataMember(Name = "decimals")]
        public uint? Decimals { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }

        [DataMember(Name = "totalSupply")]
        public string TotalSupply { get; set; }
    }
}