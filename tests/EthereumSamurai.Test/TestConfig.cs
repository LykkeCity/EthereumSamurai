using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.UnitTest
{
    public static class TestConfig
    {
        private static IServiceCollection _serviceCollection;
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void ReconfigureServices ()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddSingleton<IRpcBlockReader> (new Mocks.MockRpcBlockReader());
            _serviceCollection.AddSingleton<IIndexingService>(new Mocks.MockIndexingService());
            var loggerMock = new Moq.Mock<ILogger>();
            _serviceCollection.AddSingleton<ILogger>(loggerMock.Object);

            ServiceProvider = _serviceCollection.BuildServiceProvider();
        }
    }
}
