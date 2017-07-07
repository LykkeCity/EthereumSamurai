using EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using EthereumSamurai.Models;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;

namespace EthereumSamurai.UnitTest.Mocks
{
    public class MockIndexingService : IIndexingService
    {
        private List<BlockContent> _blocks = new List<BlockContent>();
        
        public Task<BigInteger> GetLastBlockAsync()
        {
            var block = _blocks.OrderBy(x => x.BlockModel.Number).LastOrDefault();

            return Task.FromResult(block?.BlockModel?.Number ?? 0);
        }

        public Task<BigInteger?> GetLastBlockForIndexerAsync(string indexerId)
        {
            var block = _blocks.OrderBy(x => x.BlockModel.Number).LastOrDefault(); ;

            return Task.FromResult<BigInteger?>(block?.BlockModel?.Number);
        }

        public Task IndexBlockAsync(BlockContext blockContext)
        {
            _blocks.Add(blockContext.BlockContent);

            return Task.FromResult(0);
        }
    }
}
