using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Filters;
using Lykke.Service.EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BalanceController : Controller
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [Route("getBalance/{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(BalanceResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress([FromRoute] string address)
        {
            var balance = await _balanceService.GetBalanceAsync(address.ToLowerInvariant());

            return new JsonResult(new BalanceResponse
            {
                Amount = balance.ToString()
            });
        }
    }
}