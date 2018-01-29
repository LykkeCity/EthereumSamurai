namespace Lykke.Service.EthereumSamurai.Core
{
    public static class Constants
    {
        public const string EmptyAddress = "0x0000000000000000000000000000000000000000";

        #region MongoDB_Collections

        public const string AddressHistoryCollectionName         = "AddressHistoryCollection";
        public const string BlockCollectionName                  = "BlockCollection";
        public const string BlockIndexationHistoryCollectionName = "BlockIndexationHistoryCollection";
        public const string BlockSyncedInfoCollectionName        = "BlockSyncedInfoCollectionName";
        public const string Erc20BalanceCollectionName           = "Erc20BalanceCollection";
        public const string Erc20BalanceHistoryCollectionName    = "Erc20BalanceHistoryCollection";
        public const string Erc20TransferHistoryCollectionName   = "Erc20TransferHistoryCollection";
        public const string InternalMessageCollectionName        = "InternalMessageCollection";
        public const string TransactionCollectionName            = "TransactionCollection";
        public const string Erc20ContractCollectionName          = "Erc20ContractCollection";

        #endregion

        #region ERC20

        public const string Erc20ABI = "[{\"constant\":false,\"inputs\":[{\"name\":\"_spender\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"success\",\"type\":\"bool\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"supply\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_from\",\"type\":\"address\"},{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"name\":\"success\",\"type\":\"bool\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"balance\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"name\":\"success\",\"type\":\"bool\"}],\"payable\":false,\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"},{\"name\":\"_spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"name\":\"remaining\",\"type\":\"uint256\"}],\"payable\":false,\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"}]";

        #endregion

        
    }
}
