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
using Lykke.Service.EthereumSamurai.Core.Exceptions;
using Lykke.Job.EthereumSamurai.Extensions;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class BlockIndexingActor : ReceiveActor
    {
        private readonly IBlockIndexingRole _role;
        protected Func<BigInteger, BigInteger, Messages.Common.IIndexedBlockNumberMessage> _createMessageDelegate =
            Messages.Common.CreateIndexedBlockNumberMessage;

        public BlockIndexingActor(
            IBlockIndexingRole role)
        {
            _role = role;
            FirstRun();
        }

        #region FirstRun

        public void FirstRun()
        {
            ReceiveAsync<IndexBlockMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
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
            ReceiveAsync<IndexBlockMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    var indexed = message.BlockNumber;
                    try
                    {
                        var nextToIndex = await _role.IndexBlockAsync(indexed);
                        logger.Info($"Indexed block {indexed} starting indexing {nextToIndex};");
                        Sender.Tell(_createMessageDelegate(indexed, nextToIndex));
                    }
                    catch (BlockIsNotYetMinedException exc)
                    {
                        //TODO: LOG here;
                        logger.Info($"Tip Not Yet Mined {indexed}");
                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(20), Self, message, Sender);

                        return;
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
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(2), Self, message, Sender);

                base.AroundPreRestart(cause, message);
            }
        }
    }
}