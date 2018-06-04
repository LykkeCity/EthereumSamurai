using Akka.Actor;
using Lykke.Job.EthereumSamurai.Extensions;
using Lykke.Job.EthereumSamurai.Messages.Delivery;
using System;
using System.Collections.Generic;

namespace Lykke.Job.EthereumSamurai.Actors.Delivery
{
    public class AtLeastOnceDeliveryReceiveActor<T> : ReceiveActor where T : IDeliverable
    {
        private readonly HashSet<object> _messages = new HashSet<object>();
        private readonly IActorRef _reciever;
        private readonly IActorRef _sender;
        private bool _delivered = false;
        public static TimeSpan RedeliverInterval
        {
            get { return TimeSpan.FromMinutes(2); }
        }

        public AtLeastOnceDeliveryReceiveActor(IActorRef reciever, IActorRef sender)
        {
            _reciever = reciever;
            _sender = sender;

            ReceiveAsync<T>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    logger.Info($"Proxy 1");
                    if (!_delivered)
                    {
                        logger.Info($"Proxy 2");
                        _reciever.Tell(message);
                        logger.Info($"Proxy 3");
                        Context.System.Scheduler.ScheduleTellOnce(RedeliverInterval, Self, message, Sender);
                    }
                }
            });

            ReceiveAsync<Confirm>(async (confirm) =>
            {
                using (var logger = Context.GetLogger(confirm))
                {
                    logger.Info($"Proxy 4");
                    _delivered = true;
                }
            });

            Receive<object>((message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    logger.Info($"Proxy 5");
                    //Proxy for any other messages back to the creator
                    if (!_messages.Contains(message))
                    {
                        logger.Info($"Proxy 6");
                        _messages.Add(message);
                        _sender.Tell(message);
                    }
                }
            });
        }
    }
}
