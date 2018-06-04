using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors
{
    public class BlockIndexingTipActor : BlockIndexingActor
    {
        public BlockIndexingTipActor(
            IBlockIndexingRole role) : base ()
        {
            _createMessageDelegate = (ind, next) => new Messages.Common.IndexedTipBlockNumberMessage(ind, next);
        }
    }
}
