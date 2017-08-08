using EthereumSamurai.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Repositories
{
    public interface IErc20ContractRepository
    {
        Task<Erc20ContractModel> GetAsync(string contractAddress);
        Task SaveAsync(Erc20ContractModel erc20ContractModel);
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);
        Task DeleteByHash(string trHash);
        Task SaveManyForBlockAsync(IEnumerable<Erc20ContractModel> contracts, ulong blockNumber);
    }
}
