using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class IndexingService : IIndexingService
    {
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionRepository _transactionRepository;

        public IndexingService(IBlockRepository blockRepository, ITransactionRepository transactionRepository)
        {
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task IndexBlock(BlockContent blockContent)
        {
            var blockModel = blockContent.BlockModel;
            var transactions = blockContent.Transactions;

            await _blockRepository.SaveAsync(blockModel);

            foreach (var transaction in transactions)
            {
                await _transactionRepository.SaveAsync(transaction);
            }
        }
    }
}
