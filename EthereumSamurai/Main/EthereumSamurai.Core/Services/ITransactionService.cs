using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery);
        Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash);
        Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber);
    }
}
