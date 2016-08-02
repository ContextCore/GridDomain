using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.EventChronicles
{
    class EventsReplayActor<TAggregate> : ReceivePersistentActor where TAggregate : AggregateBase
    {
        private IActorRef _waiter;
        private readonly List<object> _eventsStream = new List<object>();
        private readonly Predicate<object> _eventFilter;
        private readonly IPublisher _publisher;

        public EventsReplayActor(IPublisher publisher, Predicate<object> eventFilter)
        {
            _publisher = publisher;
            _eventFilter = eventFilter;
            PersistenceId = AggregateActorName.Parse<TAggregate>(Self.Path.Name).ToString();
            Command<Play>(c =>
            {
                _waiter = Sender;
                if (!IsRecoveryFinished) return;
                ReplayEvents();
            });

            Recover<DomainEvent>(e => _eventsStream.Add(e));
        }

        private void ReplayEvents()
        {
            foreach (var ev in _eventsStream.Where(e => _eventFilter(e)))
                _publisher.Publish(ev);

            _waiter.Tell(new PlayFinished());
        }

        public override string PersistenceId { get; }

        protected override void OnReplaySuccess()
        {
            if (_waiter == null) return;
            ReplayEvents();
            _waiter = null;
        }
    }
}