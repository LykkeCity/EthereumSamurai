using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface ITransactionService
    {
        Task<TransactionModel> GetAsync(string transactionHash);

        Task<TransactionFullInfoModel> GetFullInfoAsync(string transactionHash);

        Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery);

        Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash);

        Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber);
    }
}