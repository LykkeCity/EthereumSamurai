using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SystemController : Controller
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IRpcBlockReader  _rpcBlockReader;

        public SystemController(
            IRpcBlockReader rpcBlockReader,
            IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
            _rpcBlockReader  = rpcBlockReader;
        }

        [Route("isAlive")]
        [HttpGet]
        public async Task<IActionResult> IsAlive()
        {
            var version           = PlatformServices.Default.Application.ApplicationVersion;
            var lastRpcBlock      = await _rpcBlockReader.GetBlockCount();
            var syncedBlocksCount = await _blockRepository.GetSyncedBlocksCountAsync();

            return new JsonResult(new
            {
                syncedBlocksCount = syncedBlocksCount,
                blockchainTip     = lastRpcBlock.ToString(),
                version           = version
            });
        }
    }
}