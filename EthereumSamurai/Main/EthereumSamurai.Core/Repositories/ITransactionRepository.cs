using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Repositories
{
    public interface ITransactionRepository
    {
        Task SaveAsync(TransactionModel TransactioModel);
        Task DeleteByHash(string transactionHash);
        Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery);
        Task<TransactionModel> GetAsync(string transactionHash);
        Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber);
        Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash);
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);
        Task SaveManyForBlockAsync(IEnumerable<TransactionModel> transactionModels, ulong blockNumber);
    }
}
