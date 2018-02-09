using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Roles.Interfaces
{
    public interface IBlockIndexingRole : IActorRole
    {
        /// <summary>
        /// Index block with specified block number
        /// </summary>
        /// <param name="blockNumber"></param>
        /// <returns>Next block to index</returns>
        Task<BigInteger> IndexBlockAsync(BigInteger blockNumber);
    }
}
