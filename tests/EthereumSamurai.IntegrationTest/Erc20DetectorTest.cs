using EthereumSamurai.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EthereumSamurai.Core.Services.Erc20;

namespace EthereumSamurai.IntegrationTest
{
    [TestClass]
    public class Erc20DetectorTest
    {
        private IErc20Detector _detector;
        private IWeb3 _web3;

        [TestInitialize]
        public void TestInit()
        {
            TestConfig.ReconfigureServices();
            _detector = TestConfig.ServiceProvider.GetService<IErc20Detector>();
            _web3 = TestConfig.ServiceProvider.GetService<IWeb3>();
        }

        [TestMethod]
        public async Task Erc20DetectorTest_IsContractErc20CompatibleSuccess()
        {
            string erc20ContractAddress = "0xce2ef46ecc168226f33b6f6b8a56e90450d0d2c0";
            var isCompatible = await _detector.IsContractErc20Compatible(erc20ContractAddress);

            Assert.IsTrue(isCompatible);
        }

        [TestMethod]
        public async Task Erc20DetectorTest_IsContractErc20CompatibleFail()
        {
            string erc20ContractAddress = "0x1c4ca817d1c61f9c47ce2bec9d7106393ff981ce";
            var isCompatible = await _detector.IsContractErc20Compatible(erc20ContractAddress);

            Assert.IsFalse(isCompatible);
        }
    }
}
