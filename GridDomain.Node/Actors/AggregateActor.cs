using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.FutureEvents;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Node.Actors
{
    //TODO: extract non-actor handler to reuse in tests for aggregate reaction for command
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : ReceivePersistentActor where TAggregate : AggregateBase
    {
        private readonly IAggregateCommandsHandler<TAggregate> _handler;
        private readonly IPublisher _publisher;
        private readonly TypedMessageActor<ScheduleCommand> _schdulerActorRef;

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              AggregateFactory factory,
                              TypedMessageActor<ScheduleCommand> schdulerActorRef,
                              IPublisher publisher)
        {
            _schdulerActorRef = schdulerActorRef;
            _handler = handler;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;
            Aggregate = factory.Build<TAggregate>(AggregateActorName.Parse<TAggregate>(Self.Path.Name).Id);

            //async event came from aggregate, already raised and exists in internal aggregate collection
            Command<DomainEvent>(e => ProcessAggregateEvents());

            Command<ICommand>(cmd =>
            {
                //TODO: create more efficient way to set up a saga
                try
                {
                    Aggregate = _handler.Execute(Aggregate, cmd);
                }
                catch (Exception ex)
                {
                    _publisher.Publish(CommandFaultFactory.CreateGenericFor(cmd,ex));
                    return;
                }

                ProcessAggregateEvents(cmd.SagaId);
            });

            Recover<SnapshotOffer>(offer => Aggregate = (TAggregate) offer.Snapshot);
            Recover<DomainEvent>(e => ((IAggregate) Aggregate).ApplyEvent(e));
        }

        private void ProcessAggregateEvents(Guid? sagaId = null)
        {
            var extendedAggregate = Aggregate as Aggregate;
            if (extendedAggregate != null)
            {
                foreach (var asyncEvent in extendedAggregate.AsyncEvents)
                    asyncEvent.PipeTo(Self);
            }

            var aggregate = (IAggregate) Aggregate;

            var uncommittedEvents = aggregate.GetUncommittedEvents();

            var events = uncommittedEvents.Cast<DomainEvent>();
            if (sagaId.HasValue)
            {
                events = events.Select(e => e.CloneWithSaga(sagaId.Value));
            }

            PersistAll(events, e =>
            {
                ScheduleFutureEvent(e);
                _publisher.Publish(e);
            });
            aggregate.ClearUncommittedEvents();
        }

        private void ScheduleFutureEvent(DomainEvent e)
        {
            var futureEvent = e as FutureDomainEvent;
            if (futureEvent == null) return;

            var scheduleKey = new ScheduleKey(futureEvent.SourceId,
                $"{PersistenceId}_event_{futureEvent.SourceId}",
                $"{typeof (TAggregate).Name}_futureEvents");

            var scheduleEvent = new ScheduleCommand(new RaiseScheduledDomainEventCommand(futureEvent),
                                                    scheduleKey, 
                                                    new ExecutionOptions(futureEvent.RaiseTime,
                                                                         futureEvent.Event.GetType())
                                                    );

            _schdulerActorRef.Handle(scheduleEvent);
        }

        public TAggregate Aggregate { get; private set; }
        public override string PersistenceId { get; }
    }

 
}