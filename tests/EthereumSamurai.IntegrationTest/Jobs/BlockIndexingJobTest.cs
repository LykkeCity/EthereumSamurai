using EthereumSamurai.Indexer.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Settings;

namespace EthereumSamurai.IntegrationTest.Jobs
{
    [TestClass]
    public class BlockIndexingJobTest
    {
        public IBlockIndexingJobFactory _blockIndexingJobFactory { get; private set; }

        [TestInitialize]
        public void TestInit()
        {
            TestConfig.ReconfigureServices();
            _blockIndexingJobFactory = TestConfig.ServiceProvider.GetService<IBlockIndexingJobFactory>();
        }

        [TestMethod]
        public async Task BlockIndexingJobTest_IndexErc20Contract()
        {
            IJob job = _blockIndexingJobFactory.GetJob(new IndexingSettings()
            {
                From = 952751,
                To = 952752,
                IndexerId = "EthereumSamurai.IntegrationTest_0"
            });

            await job.RunAsync();
        }
    }
}
