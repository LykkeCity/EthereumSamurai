using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface ITransactionRepository
    {
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);

        Task DeleteByHash(string transactionHash);

        Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery);

        Task<TransactionModel> GetAsync(string transactionHash);

        Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash);

        Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber);

        Task SaveAsync(TransactionModel transactionModel);

        Task SaveManyForBlockAsync(IEnumerable<TransactionModel> transactionModels, ulong blockNumber);
    }
}