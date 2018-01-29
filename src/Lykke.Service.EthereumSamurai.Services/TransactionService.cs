using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IErc20TransferHistoryService _erc20TransferHistoryService;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository, 
            IErc20TransferHistoryService erc20TransferHistoryService)
        {
            _erc20TransferHistoryService = erc20TransferHistoryService;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery)
        {
            IEnumerable<TransactionModel> transactions = await _transactionRepository.GetAsync(transactionQuery);

            return transactions;
        }

        public async Task<TransactionModel> GetAsync(string transactionHash)
        {
            TransactionModel transaction = await _transactionRepository.GetAsync(transactionHash);

            return transaction;
        }

        public async Task<TransactionFullInfoModel> GetFullInfoAsync(string transactionHash)
        {
            TransactionModel transaction = await _transactionRepository.GetAsync(transactionHash);
            var erc20Transfers = await _erc20TransferHistoryService.GetAsync(new Erc20TransferHistoryQuery() { TransactionHash = transactionHash });

            return new TransactionFullInfoModel()
            {
                Erc20Transfers =  erc20Transfers,
                TransactionModel = transaction,
            };
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
