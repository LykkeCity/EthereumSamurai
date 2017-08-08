using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services.Erc20
{
    public interface IErc20Detector
    {
        Task<bool> IsContractErc20Compatible(string contractAddress);
    }
}
