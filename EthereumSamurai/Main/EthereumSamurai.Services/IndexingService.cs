using System;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models;
using System.Numerics;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class IndexingService : IIndexingService
    {
        private readonly IAddressHistoryRepository         _addressHistoryRepository;
        private readonly IBlockIndexationHistoryRepository _blockIndexationHistoryRepository;
        private readonly IBlockRepository                  _blockRepository;
        private readonly IBlockSyncedInfoRepository        _blockSyncedInfoRepository;
        private readonly IErc20ContractRepository          _erc20ContractRepository;
        private readonly IErc20TransferHistoryRepository   _erc20TransferHistoryRepository;
        private readonly IInternalMessageRepository        _internalMessageRepository;
        private readonly ITransactionRepository            _transactionRepository;



        public IndexingService(
            IAddressHistoryRepository         addressHistoryRepository,
            IBlockIndexationHistoryRepository blockIndexationHistoryRepository,
            IBlockRepository                  blockRepository,
            IBlockSyncedInfoRepository        blockSyncedInfoRepository,
            IErc20ContractRepository          erc20ContractRepository,
            IErc20TransferHistoryRepository   erc20TransferHistoryRepository,
            IInternalMessageRepository        internalMessageRepository,
            ITransactionRepository            transactionRepository)
        {
            _addressHistoryRepository         = addressHistoryRepository;
            _blockIndexationHistoryRepository = blockIndexationHistoryRepository;
            _blockRepository                  = blockRepository;
            _blockSyncedInfoRepository        = blockSyncedInfoRepository;
            _erc20ContractRepository          = erc20ContractRepository;
            _erc20TransferHistoryRepository   = erc20TransferHistoryRepository;
            _internalMessageRepository        = internalMessageRepository;
            _transactionRepository            = transactionRepository;
        }

        public Task<BigInteger> GetLastBlockAsync()
        {
            return _blockRepository.GetLastSyncedBlockAsync();
        }

        public async Task<BigInteger?> GetLastBlockForIndexerAsync(string indexerId)
        {
            var lastBlock = await _blockSyncedInfoRepository.GetLastSyncedBlockForIndexerAsync(indexerId);

            return lastBlock;
        }

        public async Task IndexBlockAsync(BlockContext blockContext)
        {
            var blockContent     = blockContext.BlockContent;
            var addressHistory   = blockContent.AddressHistory;
            var blockModel       = blockContent.BlockModel;
            var blockNumber      = (ulong)blockModel.Number;
            var erc20Addresses   = blockContent.CreatedErc20Contracts;
            var internalMessages = blockContent.InternalMessages;
            var transactions     = blockContent.Transactions;
            var transfers        = blockContent.Transfers;

            await _blockRepository.SaveAsync(blockModel);

            #region ProcessTransactions
            
            try
            {
                await _transactionRepository.SaveManyForBlockAsync(transactions, blockNumber);
            }
            catch
            {
                foreach (var transaction in transactions)
                {
                    var trHash = transaction.TransactionHash;

                    await _transactionRepository.DeleteByHash(trHash);
                    await _internalMessageRepository.DeleteAllForHash(trHash);
                    await _addressHistoryRepository.DeleteByHash(trHash);
                    await _erc20ContractRepository.DeleteByHash(trHash);
                    await _erc20TransferHistoryRepository.DeleteAllForHash(trHash);
                }

                await _transactionRepository.SaveManyForBlockAsync(transactions, blockNumber);
            }

            #endregion

            await _internalMessageRepository.SaveManyForBlockAsync(internalMessages, blockNumber);
            await _addressHistoryRepository.SaveManyForBlockAsync(addressHistory, blockNumber);
            await _erc20ContractRepository.SaveManyForBlockAsync(erc20Addresses, blockNumber);
            await _erc20TransferHistoryRepository.SaveForBlockAsync(transfers, blockNumber);

            //Indexer fingerPrint
            await _blockIndexationHistoryRepository.MarkBlockAsIndexed(blockNumber, blockContext.JobVersion);

            var blockSyncedInfoModel   = new EthereumSamurai.Models.Indexing.BlockSyncedInfoModel(blockContext.IndexerId, (ulong)blockModel.Number);
            
            await _blockSyncedInfoRepository.SaveAsync(blockSyncedInfoModel);
        }
    }
}
