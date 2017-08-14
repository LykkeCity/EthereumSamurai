using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Filters;
using EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class Erc20BalanceController : Controller
    {
        private readonly IErc20BalanceService _balanceService;

        public Erc20BalanceController(IErc20BalanceService balanceService)
        {
            _balanceService = balanceService;
        }


        [Route("getErc20Balance/{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(BalanceResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress([FromRoute] string address, IEnumerable<string> contracts)
        {
            address   = address.ToLowerInvariant();
            contracts = contracts.Select(x => x.ToLowerInvariant()).Distinct();

            var balances = (await _balanceService
                .GetBalances(address, contracts))
                .Select(x => new Erc20BalanceResponse
                {
                    AssetHolderAddress = x.AssetHolderAddress,
                    Balance            = x.Balance.ToString(),
                    ContractAddress    = x.ContractAddress
                })
                .ToList();

            return new JsonResult(balances);
        }
    }
}