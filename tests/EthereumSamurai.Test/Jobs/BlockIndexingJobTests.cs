using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using EthereumSamurai.Indexer.Jobs;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Numerics;
using Moq;
using Common.Log;

namespace EthereumSamurai.UnitTest.Jobs
{
    [TestClass]
    public class BlockIndexingJobTests
    {
        private BlockIndexingJob _blockIndexingJob;
        private IIndexingService _indexingService;
        private IBlockService _blockService;

        [TestInitialize]
        public void InitDependencies()
        {
            TestConfig.ReconfigureServices();
            var rpcReader = TestConfig.ServiceProvider.GetService<IRpcBlockReader>();
            _indexingService = TestConfig.ServiceProvider.GetService<IIndexingService>();
            _blockService = TestConfig.ServiceProvider.GetService<IBlockService>();
            var indexingSettings = new IndexingSettings()
            {
                IndexerId = "TestIndexer",
                From = 1,
                To = 1
            };
            var logger = new Mock<ILog>();

            _blockIndexingJob = new BlockIndexingJob(_blockService, _indexingService, indexingSettings, logger.Object, rpcReader);
        }

        [TestMethod]
        public void BlockIndexingJobTests_CompleteIndexing()
        {
            BigInteger expected = new BigInteger(1);
            Task result = _blockIndexingJob.RunAsync();
            result.Wait();
            BigInteger lastBlock =  _indexingService.GetLastBlockAsync().Result;
            Assert.AreEqual(expected, lastBlock);
        }
    }
}
