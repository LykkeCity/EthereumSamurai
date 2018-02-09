using Newtonsoft.Json;

namespace Lykke.Service.EthereumSamurai.Logger.Serialization
{
    internal class LykkeNotification
    {
        [JsonProperty(Order = 1)]
        public string Component { get; set; }

        [JsonProperty(Order = 3)]
        public LykkeLogContext Context { get; set; }

        [JsonProperty(Order = 0)]
        public string Info { get; set; }

        [JsonProperty(Order = 2)]
        public string Process { get; set; }
    }
}
