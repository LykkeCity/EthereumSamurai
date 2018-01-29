using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IErc20BalanceService
    {
        Task<IEnumerable<Erc20BalanceModel>> GetAsync(Erc20BalanceQuery query);
    }
}