using System.Linq;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;

namespace Lykke.Service.EthereumSamurai.Services.Utils
{
    public static class FilterLogExtensions
    {
        public static string GetAddressFromTopic(this FilterLog filterLog, int topicIndex)
        {
            var topicString  = filterLog.Topics[topicIndex].ToString();
            var topicBytes   = topicString.HexToByteArray();

            //// Topic is 32 Bytes DATA indexed log argument and the address has 20 bytes length;
            var addressBytes = topicBytes.Skip(12).ToArray(); 

            return addressBytes.ToHex(true);
        }
    }
}