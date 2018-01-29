using Lykke.Service.EthereumSamurai.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IErc20ContractRepository
    {
        Task<IEnumerable<Erc20ContractModel>> GetAsync(Erc20ContractQuery query);
        Task SaveAsync(Erc20ContractModel erc20ContractModel);
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);
        Task DeleteByHash(string trHash);
        Task SaveManyForBlockAsync(IEnumerable<Erc20ContractModel> contracts, ulong blockNumber);
    }
}
