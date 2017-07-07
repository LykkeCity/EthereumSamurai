using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using EthereumSamurai.Core.Services;
using System.Threading.Tasks;
using System.Numerics;

namespace EthereumSamurai.IntegrationTest
{
    [TestClass]
    public class DebugTest
    {
        private IDebug _debug;
        private IWeb3 _web3;

        [TestInitialize]
        public void TestInit()
        {
            TestConfig.ReconfigureServices();
            _debug = TestConfig.ServiceProvider.GetService<IDebug>();
            _web3 = TestConfig.ServiceProvider.GetService<IWeb3>();
        }

        [TestMethod]
        public async Task DebugTest_TraceTransaction()
        {
            string trHash = "0x9351a796b8a85d451fbae62ba26b07173134416d782026c08169dd0743cdc0e2";
            await _debug.TraceTransactionAsync(trHash, true, true, true);
        }

        [TestMethod]
        public async Task DebugTest_TraceTransactionWithStorage()
        {
            string trHash = "0x9351a796b8a85d451fbae62ba26b07173134416d782026c08169dd0743cdc0e2";
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(trHash);
            await _debug.TraceTransactionAsync(transaction.From, transaction.To, transaction.Value.Value, trHash, true, true, true);
        }
    }
}
