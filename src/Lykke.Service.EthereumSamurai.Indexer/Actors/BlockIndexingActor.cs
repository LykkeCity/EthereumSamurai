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
            //var breaker = new CircuitBreaker(
            //    maxFailures: -1,
            //    callTimeout: TimeSpan.FromMinutes(5),
            //    resetTimeout: TimeSpan.FromMinutes(1))
            //.OnOpen(NotifyMeOnOpen);

            FirstRun();
        }

        #region FirstRun

        public void FirstRun()
        {
            ReceiveAsync<IndexBlockMessage>(async (message) =>
            {
                var indexed = message.BlockNumber;
                var nextToIndex = indexed + 1;

                await _role.IndexBlockAsync(indexed);
                Sender.Tell(_createMessageDelegate(indexed, nextToIndex));

                Become(NormalState);
            });
        }

        #endregion

        #region NormalState

        public void NormalState()
        {
            ReceiveAsync<IndexBlockMessage>(async (message) =>
            {
                var indexed = message.BlockNumber;
                try
                {
                    var nextBlockToIndex = await _role.IndexBlockAsync(indexed);
                    Sender.Tell(_createMessageDelegate(indexed, nextBlockToIndex));
                }
                catch (BlockIsNotYetMinedException exc)
                {
                    //TODO: LOG here;
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(20), Self, message, Sender);

                    return;
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }

        #endregion

        public override void AroundPreRestart(Exception cause, object message)
        {
            //TODO: Log exception here
            if (message != null)
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(2), Self, message, Sender);

            base.AroundPreRestart(cause, message);
        }
    }
}