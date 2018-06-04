using System;
using System.Numerics;
using Akka.Actor;
using Autofac;
using Common;
using Lykke.Job.EthereumSamurai.Extensions;
using Lykke.Job.EthereumSamurai.Messages.Delivery;
using Lykke.Job.EthereumSamurai.ServiceLocation;
using Lykke.Service.EthereumSamurai.Core.Exceptions;
using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors
{
    public class BlockIndexingActor : ReceiveActor
    {
        private readonly IBlockIndexingRole _role;
        protected Func<BigInteger, BigInteger, Messages.Common.IIndexedBlockNumberMessage> _createMessageDelegate =
            (ind, next) => new Messages.Common.IndexedBlockNumberMessage(ind, next);

        public BlockIndexingActor()
        {
            _role = ActorLocator.Locator.Resolve<IBlockIndexingRole>();
            FirstRun();
        }

        #region FirstRun

        public void FirstRun()
        {
            ReceiveAsync<Messages.IndexBlockMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    logger.Info($"Received {message.ToJson()}");
                    Sender.Tell(new Confirm((long)message.BlockNumber));
                    var indexed = message.BlockNumber;
                    var nextToIndex = indexed + 1;

                    await _role.IndexBlockAsync(indexed);
                    logger.Info($"Indexed block {indexed} starting indexing {nextToIndex};");
                    Sender.Tell(_createMessageDelegate(indexed, nextToIndex));

                    Become(NormalState);
                }
            });
        }

        #endregion

        #region NormalState

        public void NormalState()
        {
            ReceiveAsync<Messages.IndexBlockMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    logger.Info($"Received {message.ToJson()}");
                    Sender.Tell(new Confirm((long)message.BlockNumber));
                    var indexed = message.BlockNumber;
                    try
                    {
                        var nextToIndex = await _role.IndexBlockAsync(indexed);
                        logger.Info($"Indexed block {indexed} starting indexing {nextToIndex};");
                        Sender.Tell(_createMessageDelegate(indexed, nextToIndex));
                    }
                    catch (BlockIsNotYetMinedException exc)
                    {
                        logger.Info($"Tip Not Yet Mined {indexed}");
                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(30), Self, message, Sender);
                    }
                    catch (Exception e)
                    {
                        //unexpected error
                        logger.Error(e);

                        throw;
                    }
                }
            });
        }

        #endregion

        public override void AroundPreRestart(Exception cause, object message)
        {
            using (var logger = Context.GetLogger(message))
            {
                logger.Error(cause);

                if (message != null)
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5), Self, message, Sender);

                base.AroundPreRestart(cause, message);
            }
        }
    }
}
