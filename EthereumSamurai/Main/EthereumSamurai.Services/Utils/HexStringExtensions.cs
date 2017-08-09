using System.Linq;
using Nethereum.Hex.HexConvertors.Extensions;

namespace EthereumSamurai.Services.Utils
{
    internal static class HexStringExtensions
    {
        public static string TrimLeadingZeroes(this string hex)
        {
            var source = hex.HexToByteArray();
            var result = source.SkipWhile(x => x == 0).ToArray();

            return result.ToHex(true);
        }
    }
}