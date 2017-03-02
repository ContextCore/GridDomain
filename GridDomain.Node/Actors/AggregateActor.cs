using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Logging;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Node.Actors
{
    //TODO: extract non-actor handler to reuse in tests for aggregate reaction for command
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : EventSourcedActor<TAggregate> where TAggregate : AggregateBase
    {
        public const string CreatedFault = "created fault";
        public const string CommandRaisedAnError = "command raised an error";
        public const string PublishingEvent = "Publishing event";
        public const string CommandExecutionCreatedAnEvent = "Command execution created an event";
        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly IPublisher _publisher;
        private readonly IActorRef _schedulerActorRef;

        private readonly HashSet<DomainEvent> _eventsToPersist = new HashSet<DomainEvent>();
        private readonly HashSet<Guid> _eventsToProject = new HashSet<Guid>();

        class SaveEventsAsync
        {
            public SaveEventsAsync(DomainEvent[] events, Action<DomainEvent> act, Action continuation, Aggregate state)
            {
                Continuation = continuation;
                State = state;
                Events = events;
                Act = act;
            }

            public Action Continuation { get; }
            public DomainEvent[] Events { get; }
            public Aggregate State { get; }
            public Action<DomainEvent> Act { get; }
        }

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              IActorRef schedulerActorRef,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(aggregateConstructor, snapshotsPersistencePolicy)
        {
            _publisher = publisher;
            _customHandlersActor = customHandlersActor;
            _schedulerActorRef = schedulerActorRef;
            _domainEventProcessEntry = new ProcessEntry(Self.Path.Name, PublishingEvent, CommandExecutionCreatedAnEvent);
            _domainEventProcessFailEntry = new ProcessEntry(Self.Path.Name, CreatedFault, CommandRaisedAnError);

            ((Aggregate) State).RegisterPersistence((evtTask, onEventPersist, continuation, newState) =>
                                                        evtTask.ContinueWith(t => new SaveEventsAsync(t.Result, onEventPersist, continuation, newState))
                                                               .PipeTo(Self));

            BecomeStacked(() => ReceivingCommands(handler));
        }

        private void ReceivingCommands(IAggregateCommandsHandler<TAggregate> handler)
        {
            Command<IMessageMetadataEnvelop<ICommand>>(m =>
                                                       {
                                                           var cmd = m.Message;
                                                           Monitor.IncrementMessagesReceived();
                                                           Log.Debug("{Aggregate} received a {@command}", PersistenceId, cmd);

                                                           handler.ExecuteAsync((TAggregate) State, cmd);

                                                           var eventsMetadata = m.Metadata.CreateChild(Id, _domainEventProcessEntry);
                                                           BecomeStacked(() => ProcessingCommand(m, eventsMetadata));
                                                       });
        }

        private void ProcessingCommand(IMessageMetadataEnvelop<ICommand> commandEnvelop, MessageMetadata eventsMetadata)
        {
            CommandAny(c => c.Match()
                             //finished some call on aggregate, need persist produced events
                             .With<SaveEventsAsync>(e =>
                                                    {
                                                        int count = e.Events.Length;
                                                        var eventsToSave = e.Events
                                                                            .Select(ev =>
                                                                                    {
                                                                                        var eventWithSaga = ev.CloneWithSaga(commandEnvelop.Message.SagaId);
                                                                                        _eventsToPersist.Add(eventWithSaga);
                                                                                        return eventWithSaga;
                                                                                    });
                                                        PersistAll(eventsToSave,
                                                                   o =>
                                                                   {
                                                                       e.State.MarkPersisted(o);

                                                                       OnEventPersisted(o, eventsMetadata);

                                                                       e.Act(o);
                                                                       if (--count == 0)
                                                                       {
                                                                           e.Continuation();
                                                                       }

                                                                       if (!e.State.EventToPersist.Any())
                                                                       {
                                                                           //command execution is finished
                                                                           Self.Tell(e.State);
                                                                       }
                                                                   });
                                                    })
                             //aggregate raised an error during command execution
                             .With<Failure>(f =>
                                            {
                                                ProjectFault(commandEnvelop.Message, f.Exception, commandEnvelop.Metadata);

                                                FinishCommandExecution();
                                            })
                             // .With<Status.Failure>()
                             //aggregate command execution is finished 
                             //produced events are persisted
                             //we can have events not projected yet
                             .With<Aggregate>(newState =>
                                              {
                                                  if (newState.EventToPersist.Any())
                                                      throw new UncommitedStateExeption();

                                                  //can renew state now, as we are waiting only for event projections
                                                  State = newState;
                                                  FinishCommandExecution();
                                              })
                             //projection of event pack from aggregate finished
                             //we can have more event packs to project
                             .With<AllHandlersCompleted>(m =>
                                                         {
                                                             OnHandlersFinish(commandEnvelop.Metadata, m);
                                                             //all projections are finished, aggregate events are persisted
                                                             //it means command execution is finished
                                                             if (!_eventsToProject.Any() && !_eventsToPersist.Any())
                                                                 FinishCommandExecution();
                                                         })
                             .With<IMessageMetadataEnvelop<ICommand>>(o => Stash.Stash())
                             .Default(e => { throw new UnknownMessageReceivedException(); })
                      );
        }

        private void FinishCommandExecution()
        {
            UnbecomeStacked();
            Stash.UnstashAll();
        }

        protected override void Terminating()
        {
            Command<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           Self.Tell(CancelShutdownRequest.Instance);
                                                           Stash.Stash();
                                                       });
            base.Terminating();
        }

        private void OnHandlersFinish(IMessageMetadata commandMetadata, AllHandlersCompleted processComplete)
        {
            if (!_eventsToProject.Remove(processComplete.ProjectId))
                throw new UnknownProjectionFinishedException();

            foreach (var e in processComplete.DomainEvents)
            {
                var eventMetadata = commandMetadata.CreateChild(e.SourceId, _domainEventProcessEntry);
                _publisher.Publish(e, eventMetadata);
            }

            if (processComplete.Fault == null)
                return;

            var faultMetadata = commandMetadata.CreateChild(commandMetadata.MessageId, _domainEventProcessFailEntry);
            _publisher.Publish(processComplete.Fault, faultMetadata);
        }

    
        private void OnEventPersisted(DomainEvent evt, IMessageMetadata commandMetadata)
        {
            TrySaveSnapshot();

            NotifyPersistenceWatchers(evt);

            ProjectEvents(evt, commandMetadata);
        }

        private void ProjectFault(ICommand cmd, Exception ex, IMessageMetadata commandMetadata)
        {
            var fault = Fault.NewGeneric(cmd, ex, cmd.SagaId, typeof(TAggregate));

            var faultMetadata = commandMetadata.CreateChild(cmd.Id, _domainEventProcessFailEntry);

            _customHandlersActor.Ask<AllHandlersCompleted>(new MessageMetadataEnvelop<IFault>(fault, faultMetadata))
                                .PipeTo(Self);

            Log.Error(ex, "{Aggregate} raised an error {@Exception} while executing {@Command}", PersistenceId, ex, cmd);
        }


        private void ProjectEvents(DomainEvent evt, IMessageMetadata commandMetadata)
        {
            var envelop = new MessageMetadataEnvelop<Project>(new Project(new[] {evt}), commandMetadata);

            _eventsToProject.Add(envelop.Message.ProjectId);

            evt.Match()
               .With<FutureEventScheduledEvent>(m => Handle(m, commandMetadata))
               .With<FutureEventCanceledEvent>(m => Handle(m, commandMetadata));

            _customHandlersActor.Ask<AllHandlersCompleted>(envelop).PipeTo(Self);
        }

        public void Handle(FutureEventScheduledEvent futureEventScheduledEvent, IMessageMetadata messageMetadata)
        {
            var message = futureEventScheduledEvent;
            var scheduleId = message.Id;
            var aggregateId = message.Event.SourceId;

            var description = $"Aggregate {typeof(TAggregate).Name} id = {aggregateId} scheduled future event "
                              + $"{scheduleId} with payload type {message.Event.GetType().Name} on time {message.RaiseTime}\r\n"
                              + $"Future event: {message.ToPropsString()}";

            var scheduleKey = CreateScheduleKey(scheduleId, aggregateId, description);

            var command = new RaiseScheduledDomainEventCommand(message.Id, message.SourceId, Guid.NewGuid());
            var metadata = messageMetadata.CreateChild(command.Id,
                                                       new ProcessEntry(GetType().Name,
                                                                        "Scheduling raise future event command",
                                                                        "FutureEventScheduled event occured"));

            var confirmationEventType = typeof(IMessageMetadataEnvelop<>).MakeGenericType(message.Event.GetType());

            var scheduleEvent = new ScheduleCommand(command,
                                                    scheduleKey,
                                                    new ExtendedExecutionOptions(message.RaiseTime,
                                                                                 confirmationEventType,
                                                                                 message.Event.SourceId,
                                                                                 nameof(DomainEvent.SourceId)),
                                                    metadata);

            _schedulerActorRef.Tell(scheduleEvent);
        }

        public static ScheduleKey CreateScheduleKey(Guid scheduleId, Guid aggregateId, string description)
        {
            return new ScheduleKey(scheduleId,
                                   $"{typeof(TAggregate).Name}_{aggregateId}_future_event_{scheduleId}",
                                   $"{typeof(TAggregate).Name}_futureEvents",
                                   "");
        }

        public void Handle(FutureEventCanceledEvent futureEventCanceledEvent, IMessageMetadata metadata)
        {
            var message = futureEventCanceledEvent;
            var key = CreateScheduleKey(message.FutureEventId, message.SourceId, "");
            var unscheduleMessage = new Unschedule(key);
            _schedulerActorRef.Tell(unscheduleMessage);
        }
    }

    internal class UnknownMessageReceivedException : Exception {}

    internal class UnknownProjectionFinishedException : Exception {}

    internal class UncommitedStateExeption : Exception {}
}