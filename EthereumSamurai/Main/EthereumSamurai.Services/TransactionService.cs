using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery)
        {
            IEnumerable<TransactionModel> transactions = await _transactionRepository.GetAsync(transactionQuery);

            return transactions;
        }

        public async Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash)
        {
            IEnumerable<TransactionModel> transactions = await _transactionRepository.GetForBlockHashAsync(blockHash);

            return transactions;
        }

        public async Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber)
        {
            IEnumerable<TransactionModel> transactions = await _transactionRepository.GetForBlockNumberAsync(blockNumber);

            return transactions;
        }
    }
}
