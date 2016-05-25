using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging
{
    /// <summary>
    /// Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate>: PersistentActor where TAggregate : AggregateBase
    {
        private TAggregate _aggregate;
        private readonly IPublisher _publisher;
        private readonly IAggregateCommandsHandler<TAggregate> _handler;
        public override string PersistenceId { get; }

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler, AggregateFactory factory, IPublisher publisher)
        {
            _handler = handler;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;
            _aggregate = factory.Build<TAggregate>(AggregateActorName.Parse<TAggregate>(Self.Path.Name).Id);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                _aggregate = (TAggregate) (message as SnapshotOffer).Snapshot;
            }
            else ((IAggregate)_aggregate).ApplyEvent(message);
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            var events = _handler.Execute(_aggregate, (ICommand) message);
            PersistAll(events, e => _publisher.Publish(e));
            return true;
        }
    }
}