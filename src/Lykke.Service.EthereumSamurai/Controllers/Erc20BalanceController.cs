using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Filters;
using Lykke.Service.EthereumSamurai.Models.Query;
using Lykke.Service.EthereumSamurai.Requests;
using Lykke.Service.EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Controllers
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


        [Route("getErc20Balance")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<Erc20BalanceResponse>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> Get([FromBody] GetErc20BalanceRequest request, [FromQuery] int? start, [FromQuery] int? count)
        {
            var query = new Erc20BalanceQuery
            {
                AssetHolder = request.AssetHolder?.ToLowerInvariant(),
                BlockNumber = request.BlockNumber,
                Contracts   = request.Contracts?.Select(x => x.ToLowerInvariant()).Distinct(),
                Count       = count,
                Start       = start
            };

            return await GetAsync(query);
        }

        [Route("getErc20Balance/{address}")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<Erc20BalanceResponse>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress([FromRoute] string address, [FromBody] IEnumerable<string> contracts)
        {
            var query = new Erc20BalanceQuery
            {
                AssetHolder = address.ToLowerInvariant(),
                Contracts   = contracts?.Select(x => x.ToLowerInvariant()).Distinct()
            };

            return await GetAsync(query);
        }

        private async Task<IActionResult> GetAsync(Erc20BalanceQuery query)
        {
            var balances = (await _balanceService.GetAsync(query))
                .Select(x => new Erc20BalanceResponse
                {
                    AssetHolderAddress = x.AssetHolderAddress,
                    Balance            = x.Balance.ToString(),
                    BlockNumber        = x.BlockNumber,
                    ContractAddress    = x.ContractAddress
                })
                .ToList();

            return new JsonResult(balances);
        }
    }
}