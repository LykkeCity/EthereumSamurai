using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Filters;
using EthereumSamurai.Models.Query;
using EthereumSamurai.Requests;
using EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class Erc20TransferHistoryController : Controller
    {
        private readonly IErc20TransferHistoryService _transferHistoryService;

        public Erc20TransferHistoryController(IErc20TransferHistoryService transferHistoryService)
        {
            _transferHistoryService = transferHistoryService;
        }

        [HttpPost("getErc20Transfers")]
        [ProducesResponseType(typeof(IEnumerable<Erc20TransferHistoryResponse>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> Get([FromBody] GetErc20TransferHistoryRequest request, [FromQuery] int? start, [FromQuery] int? count)
        {
            var query = new Erc20TransferHistoryQuery
            {
                AssetHolder     = request.AssetHolder,
                BlockNumber     = request.BlockNumber,
                Contracts       = request.Contracts,
                Count           = count,
                Start           = start
            };

            var transfers = (await _transferHistoryService.GetAsync(query))
                .Select(x => new Erc20TransferHistoryResponse
                {
                    BlockHash        = x.BlockHash,
                    BlockNumber      = x.BlockNumber,
                    BlockTimestamp   = x.BlockTimestamp,
                    ContractAddress  = x.ContractAddress,
                    From             = x.From,
                    LogIndex         = x.LogIndex,
                    To               = x.To,
                    TransactionHash  = x.TransactionHash,
                    TransactionIndex = x.TransactionIndex,
                    TransferAmount   = x.TransferAmount.ToString()
                })
                .ToList();

            return new JsonResult(transfers);
        }
    }
}