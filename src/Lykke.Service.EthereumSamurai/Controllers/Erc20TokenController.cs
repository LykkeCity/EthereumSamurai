using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Filters;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using Lykke.Service.EthereumSamurai.Requests;
using Lykke.Service.EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class Erc20TokenController : Controller
    {
        private readonly IErc20ContractService _contractService;

        public Erc20TokenController(
            IErc20ContractService contractService)
        {
            _contractService = contractService;
        }

        [Route("{contractAddress}")]
        [HttpGet]
        [ProducesResponseType(typeof(Erc20TokenResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> Get(string contractAddress)
        {
            var tokens = await QueryServiceAsync(new Erc20ContractQuery
            {
                Address = contractAddress
            });

            return new JsonResult(tokens.FirstOrDefault());
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Erc20TokenResponse>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> Get(GetErc20TokenRequest request)
        {
            var tokens = await QueryServiceAsync(new Erc20ContractQuery
            {
                NameOrSymbol = request.Query,
                Count        = request.Count,
                Start        = request.Start
            });

            return new JsonResult(tokens);
        }

        private async Task<IEnumerable<Erc20TokenResponse>> QueryServiceAsync(Erc20ContractQuery query)
        {
            return (await _contractService.GetAsync(query))
                .Select(x => new Erc20TokenResponse
                {
                    ContractAddress = x.Address,
                    Decimals        = x.TokenDecimals,
                    Name            = x.TokenName,
                    Symbol          = x.TokenSymbol,
                    TotalSupply     = x.TokenTotalSupply.ToString()
                });
        }
    }
}