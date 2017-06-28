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
using EthereumSamurai.Core.Repositories;

namespace EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SystemController : Controller
    {
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IBlockRepository _blockRepository;

        public SystemController(IRpcBlockReader rpcBlockReader, IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
            _rpcBlockReader = rpcBlockReader;
        }

        [Route("isAlive")]
        [HttpGet]
        public async Task<IActionResult> IsAlive()
        {
            string version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
            BigInteger lastRpcBlock = await _rpcBlockReader.GetBlockCount();
            BigInteger syncedBlocksCount = await _blockRepository.GetSyncedBlocksCountAsync();

            return new JsonResult(new
            {
                syncedBlocksCount = syncedBlocksCount,
                blockchainTip = lastRpcBlock.ToString(),
                version = version
            });
        }
    }
}
