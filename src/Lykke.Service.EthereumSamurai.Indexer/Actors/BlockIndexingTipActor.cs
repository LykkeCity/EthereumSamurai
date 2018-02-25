using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Models;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models;
using Common.Log;
using Akka.Actor;
using Lykke.Job.EthereumSamurai.Messages;
using static Lykke.Job.EthereumSamurai.Messages.BlockIndexingActor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using static Akka.Actor.Status;
using Akka.Pattern;
using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class BlockIndexingTipActor : BlockIndexingActor
    {
        public BlockIndexingTipActor(
            IBlockIndexingRole role) : base (role)
        {
            _createMessageDelegate = (ind, next) => new Messages.Common.IndexedTipBlockNumberMessage(ind, next);
        }
    }
}
