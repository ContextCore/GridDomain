using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Logging;
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
        private readonly TypedMessageActor<ScheduleCommand> _schedulerActorRef;

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              AggregateFactory factory,
                              TypedMessageActor<ScheduleCommand> schedulerActorRef,
                              IPublisher publisher)
        {
            _schedulerActorRef = schedulerActorRef;
            _handler = handler;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;
            Aggregate = factory.Build<TAggregate>(AggregateActorName.Parse<TAggregate>(Self.Path.Name).Id);

       
            //async aggregate method execution finished, aggregate already raised events
            //need process it in usual way
            Command<AsyncEventsRecieved>(m =>
            {
                if (m.Exception != null)
                {
                   _publisher.Publish(CommandFaultFactory.CreateGenericFor(m.Command, m.Exception));
                    return;
                }

                (Aggregate as Aggregate).FinishAsyncExecution(m.InvocationId);
                ProcessAggregateEvents(m.Command);
            });

            Command<ICommand>(cmd =>
            {
                try
                {
                    Aggregate = _handler.Execute(Aggregate, cmd);
                }
                catch (Exception ex)
                {
                    _publisher.Publish(CommandFaultFactory.CreateGenericFor(cmd,ex));
                    return;
                }

                ProcessAggregateEvents(cmd);
            });

            Recover<SnapshotOffer>(offer => Aggregate = (TAggregate) offer.Snapshot);
            Recover<DomainEvent>(e => ((IAggregate) Aggregate).ApplyEvent(e));
        }

        private void ProcessAggregateEvents(ICommand command)
        {

            var aggregate = (IAggregate) Aggregate;

            var uncommittedEvents = aggregate.GetUncommittedEvents();

            var events = uncommittedEvents.Cast<DomainEvent>();
            if (command.SagaId != Guid.Empty)
            {
                events = events.Select(e => e.CloneWithSaga(command.SagaId));
            }

            PersistAll(events, e =>
            {
                e.Match().With<FutureDomainEvent>(ScheduleFutureEvent);
                _publisher.Publish(e);
            });
            aggregate.ClearUncommittedEvents();

            ProcessAsyncMethods(command);
        }

        private void ProcessAsyncMethods(ICommand command)
        {
            var extendedAggregate = Aggregate as Aggregate;
            if (extendedAggregate == null) return;

            //When aggregate notifies external world about async method execution start,
            //actor should schedule results to process it
            //command is included to safe access later, after async execution complete
            var cmd = command;
            foreach (var asyncMethod in extendedAggregate.AsyncUncomittedEvents)
                asyncMethod.ResultProducer.ContinueWith(t => new AsyncEventsRecieved(t.IsFaulted ? null: t.Result, cmd, asyncMethod.InvocationId, t.Exception))
                                          .PipeTo(Self);

            extendedAggregate.AsyncUncomittedEvents.Clear();
        }

        private void ScheduleFutureEvent(FutureDomainEvent futureEvent)
        {
            
            var scheduleKey = new ScheduleKey(futureEvent.Id,
                $"{typeof(TAggregate).Name}_{PersistenceId}_future_event_{futureEvent.Id}",
                $"{typeof(TAggregate).Name}_futureEvents",
                $"Aggregate {typeof(TAggregate).Name} id = {futureEvent.Event.SourceId} scheduled future event " +
                $"{futureEvent.Id} with payload type {futureEvent.Event.GetType().Name} on time {futureEvent.RaiseTime}\r\n" +
                $"Future event: {futureEvent.ToPropsString()}");

            var scheduleEvent = new ScheduleCommand(new RaiseScheduledDomainEventCommand(futureEvent.Id,futureEvent.SourceId),
                                                    scheduleKey, 
                                                    new ExecutionOptions(futureEvent.RaiseTime,
                                                                         futureEvent.Event.GetType())
                                                    );

            _schedulerActorRef.Handle(scheduleEvent);
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        public TAggregate Aggregate { get; private set; }
        public override string PersistenceId { get; }
    }

 
}