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

namespace EthereumSamurai.Test.Jobs
{
    [TestClass]
    public class BlockIndexingJobTests
    {
        private BlockIndexingJob _blockIndexingJob;
        private IIndexingService _indexingService;

        [TestInitialize]
        public void InitDependencies()
        {
            TestConfig.ReconfigureServices();
            var rpcReader = TestConfig.ServiceProvider.GetService<IRpcBlockReader>();
            _indexingService = TestConfig.ServiceProvider.GetService<IIndexingService>();
            var indexingSettings = new IndexingSettings()
            {
                IndexerId = "TestIndexer",
                From = 1,
                To = 1
            };
            var logger = TestConfig.ServiceProvider.GetService<ILogger>();

            _blockIndexingJob = new BlockIndexingJob(rpcReader, _indexingService, indexingSettings, logger);
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
