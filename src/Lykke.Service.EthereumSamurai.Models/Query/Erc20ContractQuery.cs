namespace Lykke.Service.EthereumSamurai.Models.Query
{
    public class Erc20ContractQuery
    {
        public string Address { get; set; }

        public int? Count { get; set; }

        public string NameOrSymbol { get; set; }

        public int? Start { get; set; }
    }
}