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

        public IndexingService(IBlockRepository blockRepository, 
            ITransactionRepository transactionRepository, 
            IBlockSyncedInfoRepository blockSyncedInfoRepository)
        {
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
            _blockSyncedInfoRepository = blockSyncedInfoRepository;
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

            await _blockRepository.SaveAsync(blockModel);
            await _transactionRepository.DeleteAllForBlockNumberAsync((ulong)blockModel.Number);
            foreach (var transaction in transactions)
            {
                await _transactionRepository.SaveAsync(transaction);
            }

            //Indexer fingerPrint
            var blockSyncedInfoModel = new EthereumSamurai.Models.Indexing.BlockSyncedInfoModel(blockContext.IndexerId, (ulong)blockModel.Number);
            await _blockSyncedInfoRepository.SaveAsync(blockSyncedInfoModel);
        }
    }
}
