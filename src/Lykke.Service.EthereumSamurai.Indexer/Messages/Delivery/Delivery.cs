using System.ComponentModel;

namespace Lykke.Job.EthereumSamurai.Messages.Delivery
{
    [ImmutableObject(true)]
    public class Confirm
    {
        public Confirm(long deliveryId)
        {
            DeliveryId = deliveryId;
        }

        public long DeliveryId { get; }
    }

    public interface IDeliverable
    {
        long DiliveryId { get; }
    }
}
