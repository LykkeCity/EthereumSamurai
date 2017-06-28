using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Query;
using EthereumSamurai.Models.Blockchain;
using AutoMapper;
using EthereumSamurai.Responses;
using EthereumSamurai.Requests;
using EthereumSamurai.Filters;
using System.Numerics;

namespace EthereumSamurai.Controllers
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
            address = address.ToLower();
            BigInteger balance = await _balanceService.GetBalanceAsync(address);

            return new JsonResult(new BalanceResponse()
            {
                Amount = balance.ToString()
            });
        }
    }
}
