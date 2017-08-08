namespace EthereumSamurai.Core
{
    public static class Constants
    {
        public const string EmptyAddress = "0x0000000000000000000000000000000000000000";

        #region MongoDB_Collections

        public const string AddressHistoryCollectionName  = "AddressHistoryCollection";
        public const string BlockCollectionName           = "BlockCollection";
        public const string BlockSyncedInfoCollectionName = "BlockSyncedInfoCollectionName";
        public const string InternalMessageCollectionName = "InternalMessageCollection";
        public const string TransactionCollectionName     = "TransactionCollection";

        #endregion
    }
}
