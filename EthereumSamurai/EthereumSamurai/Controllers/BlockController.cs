using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Filters;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using EthereumSamurai.Requests;
using EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BlockController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBlockService _blockService;

        public BlockController(
            IBlockService blockService,
            IMapper mapper)
        {
            _blockService = blockService;
            _mapper = mapper;
        }

        [Route("number/{blockNumber}")]
        [HttpGet]
        [ProducesResponseType(typeof(BlockResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForBlockNumber([FromRoute] ulong blockNumber)
        {
            var blockModel = await _blockService.GetForBlockNumberAsync(blockNumber);

            return ProcessResponse(blockModel);
        }

        [Route("hash/{blockHash}")]
        [HttpGet]
        [ProducesResponseType(typeof(BlockResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForBlockNumber([FromRoute] string blockHash)
        {
            var blockModel = await _blockService.GetForHashAsync(blockHash);

            return ProcessResponse(blockModel);
        }

        private IActionResult ProcessResponse(BlockModel model)
        {
            return new JsonResult(_mapper.Map<BlockResponse>(model));
        }
    }
}