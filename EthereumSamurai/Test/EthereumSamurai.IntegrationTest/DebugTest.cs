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
        public async Task DebugTest_TraceTransactionForContractCreation()
        {
            string trHash = "0x3685d6a6c2c6b27c846fc54b48886e14b3cfde6101973466359474fc27982395";//"0x9351a796b8a85d451fbae62ba26b07173134416d782026c08169dd0743cdc0e2";
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(trHash);
            var transactionReciept = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(trHash);

            var result = await _debug.TraceTransactionAsync(transaction.From, transaction.To, transactionReciept.ContractAddress,
                transaction.Value.Value, trHash, false, true, false);
        }
        

        [TestMethod]
        public async Task DebugTest_TraceTransactionForComplexTransfer()
        {
            string trHash = "0x76e8ca60544f12a27414345f06a7d3bd723ca7ac630d728aeb358e3b4a9d31dd";
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(trHash);
            var transactionReciept = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(trHash);

            var result = await _debug.TraceTransactionAsync(transaction.From, transaction.To, transactionReciept.ContractAddress,
                transaction.Value.Value, trHash, false, true, false);
        }

        [TestMethod]
        public async Task DebugTest_TraceTransactionForBigBigTrace()
        {
            string trHash = "0x41ab6e34dc167c64a7cb4bc7be1b1b811b29472507559b6377b7098945034f57";
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(trHash);
            var transactionReciept = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(trHash);

            var result = await _debug.TraceTransactionAsync(transaction.From, transaction.To, transactionReciept.ContractAddress,
                transaction.Value.Value, trHash, false, true, false);
        }

        [TestMethod]
        public async Task DebugTest_TraceTransactionForTransfer()
        {
            string trHash = "0xae43914fd28467c8eccb706f3c8b2a6c0e69e231a490e79acc35b82d567572ad";//"0x9351a796b8a85d451fbae62ba26b07173134416d782026c08169dd0743cdc0e2";
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(trHash);
            var transactionReciept = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(trHash);

            var result = await _debug.TraceTransactionAsync(transaction.From, transaction.To, transactionReciept.ContractAddress,
                transaction.Value.Value, trHash, false, true, false);
        }

        [TestMethod]
        public async Task DebugTest_TraceTransactionWithError()
        {
            string trHash = "0x1e43fac02bc009730edac69ab6ed0b498cef40908083ae7f53793490950256e3";
            var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(trHash);

            var result = await _debug.TraceTransactionAsync(transaction.From, transaction.To, null, transaction.Value.Value, trHash, false, true, false);
        }
    }
}
