using Lykke.Service.EthereumSamurai.Models.Indexing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Roles.Interfaces
{
    public interface IBlockIndexingDispatcherRole : IActorRole
    {
        Task<JobInfo> RetreiveJobInfoAsync();

        Task<IEnumerable<ulong>> RetreiveMiddleBlocksToIndex(int take = 1000);
    }
}
