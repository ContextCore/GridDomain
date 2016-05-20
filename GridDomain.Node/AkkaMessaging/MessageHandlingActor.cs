using System;
using Akka.Actor;
using Akka.IO;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{

    public class AggregateActor<TAggregate>: PersistentActor where TAggregate : AggregateBase
    {
        private TAggregate _aggregate;

        public AggregateActor(Guid id, AggregateFactory factory)
        {
            PersistenceId = typeof (TAggregate).Name + id;
            _aggregate = factory.Build<TAggregate>(id);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                _aggregate = (TAggregate) (message as SnapshotOffer).Snapshot;
            }
            else
            ((IAggregate)_aggregate).ApplyEvent(message);
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            throw new NotImplementedException();
        }

        public override string PersistenceId { get; } 
    }

    public class MessageHandlingActor<TMessage, THandler> : UntypedActor where THandler : IHandler<TMessage>
    {
        private readonly THandler _handler;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public MessageHandlingActor(THandler handler)
        {
            _handler = handler;
            _log.Trace($"Created message handler actor {GetType()}");
        }

        protected override void OnReceive(object msg)
        {
            _log.Trace($"Handler actor got message: {msg.ToPropsString()}");
            _handler.Handle((TMessage) msg);
        }
    }
}