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
        Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery);
        Task<TransactionModel> GetAsync(string transactionHash);
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);
    }
}
