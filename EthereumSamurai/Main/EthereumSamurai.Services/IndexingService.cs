using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class IndexingService : IIndexingService
    {
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBlockSyncedInfoRepository _blockSyncedInfoRepository;
        private readonly IInternalMessageRepository _internalMessageRepository;

        public IndexingService(IBlockRepository blockRepository,
            ITransactionRepository transactionRepository,
            IBlockSyncedInfoRepository blockSyncedInfoRepository,
            IInternalMessageRepository internalMessageRepository)
        {
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
            _blockSyncedInfoRepository = blockSyncedInfoRepository;
            _internalMessageRepository = internalMessageRepository;
        }

        public Task<BigInteger> GetLastBlockAsync()
        {
            return _blockRepository.GetLastSyncedBlockAsync();
        }

        public async Task<BigInteger?> GetLastBlockForIndexerAsync(string indexerId)
        {
            BigInteger? lastBlock = await _blockSyncedInfoRepository.GetLastSyncedBlockForIndexerAsync(indexerId);

            return lastBlock;
        }

        public async Task IndexBlockAsync(BlockContext blockContext)
        {
            var blockContent = blockContext.BlockContent;
            var blockModel = blockContent.BlockModel;
            var transactions = blockContent.Transactions;
            var internalMessages = blockContent.InternalMessages;
            ulong blockNumber = (ulong)blockModel.Number;

            await _blockRepository.SaveAsync(blockModel);
            await _transactionRepository.SaveManyForBlockAsync(transactions, blockNumber);
            await _internalMessageRepository.SaveManyForBlockAsync(internalMessages, blockNumber);
            //Indexer fingerPrint
            var blockSyncedInfoModel = new EthereumSamurai.Models.Indexing.BlockSyncedInfoModel(blockContext.IndexerId, (ulong)blockModel.Number);
            await _blockSyncedInfoRepository.SaveAsync(blockSyncedInfoModel);
        }
    }
}
