using System.Numerics;

namespace EthereumSamurai.Models.Blockchain
{
    public class Erc20BalanceModel
    {
        public string AssetHolderAddress { get; set; }

        public BigInteger Balance { get; set; }

        public ulong BlockNumber { get; set; }
        
        public string ContractAddress { get; set; }
    }
}