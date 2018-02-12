using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IErc20BalanceRepository
    {
        Task<IEnumerable<Erc20BalanceModel>> GetAsync(Erc20BalanceQuery query);

        Task SaveForBlockAsync(IEnumerable<Erc20BalanceModel> balances, ulong blockNumber);
    }
}