using System;
using Common.Log;

namespace Lykke.Service.EthereumSamurai.Client
{
    public class EthereumSamuraiClient : IEthereumSamuraiClient, IDisposable
    {
        private readonly ILog _log;

        public EthereumSamuraiClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
