using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IRpcBlockReader _rpcBlockReader;

        public IsAliveController(
            IRpcBlockReader rpcBlockReader,
            IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
            _rpcBlockReader = rpcBlockReader;
        }

        [HttpGet]
        public async Task<IActionResult> IsAlive()
        {
            var version = PlatformServices.Default.Application.ApplicationVersion;
            var lastRpcBlock = await _rpcBlockReader.GetBlockCount();
            var syncedBlocksCount = await _blockRepository.GetSyncedBlocksCountAsync();

            return new JsonResult(new
            {
                syncedBlocksCount = syncedBlocksCount,
                blockchainTip = lastRpcBlock.ToString(),
                version = version
            });
        }
    }
}
