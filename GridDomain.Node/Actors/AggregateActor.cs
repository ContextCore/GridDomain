using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;

using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
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
    public class AggregateActor<TAggregate> : EventSourcedActor<TAggregate> where TAggregate : Aggregate
    {
        public const string CreatedFault = "created fault";
        public const string CommandRaisedAnError = "command raised an error";
        public const string PublishingEvent = "Publishing event";
        public const string CommandExecutionCreatedAnEvent = "Command execution created an event";

        private const string ErrorOnEventApplyText = "Aggregate {id} raised errors on events apply after persist while executing command {@command}  \r\n" +
                                                     "State is supposed to be corrupted.  \r\n" +
                                                     "Events will be persisted.\r\n" +
                                                     "Aggregate will be stopped immediately, all pending commands will be dropped.";

        const string ErrorOnContinuationText = "Aggregate {id} raised error while executing command {@command}. \r\n" +
                                               "After some events produced and persisted, a continuation raises an error \r\n" +
                                               "Current aggregate state will be taken as a new state. \r\n" +
                                               "Aggregate is running and will execute futher commands";

        private readonly IActorRef _customHandlersActor;
        private readonly ProcessEntry _domainEventProcessEntry;
        private readonly ProcessEntry _domainEventProcessFailEntry;
        private readonly IPublisher _publisher;
        private readonly IActorRef _schedulerActorRef;
        private bool _shouldTerminate;

        private readonly IDictionary<Guid, object> _messagesToProject = new Dictionary<Guid, object>();
        private bool IsProjecting => _messagesToProject.Any();

        private readonly IAggregateCommandsHandler<TAggregate> _aggregateCommandsHandler;
        private readonly List<IActorRef> _commandCompletedWaiters = new List<IActorRef>();
        private AggregateBase UncomfirmedState { get; set; }
        private Action StateConfirmedContinuation { get; set; }

        public new TAggregate State
        {
            get { return (TAggregate) base.State; }
            private set { base.State = value; }
        }

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              IActorRef schedulerActorRef,
                              IPublisher publisher,
                              ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                              IConstructAggregates aggregateConstructor,
                              IActorRef customHandlersActor) : base(aggregateConstructor, snapshotsPersistencePolicy)
        {
            _aggregateCommandsHandler = handler;
            _publisher = publisher;
            _customHandlersActor = customHandlersActor;
            _schedulerActorRef = schedulerActorRef;
            _domainEventProcessEntry = new ProcessEntry(Self.Path.Name, PublishingEvent, CommandExecutionCreatedAnEvent);
            _domainEventProcessFailEntry = new ProcessEntry(Self.Path.Name, CreatedFault, CommandRaisedAnError);


            RegisterAggregatePersistence();

            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
        }

        private void RegisterAggregatePersistence()
        {
            State.RegisterPersistence((newStateTask, continuation) =>
                                          newStateTask.ContinueWith(t =>
                                                                    {
                                                                        UncomfirmedState = t.Result;
                                                                        StateConfirmedContinuation = continuation;
                                                                        return new SaveEventsAsync(t.Result, continuation);
                                                                    })
                                                      .PipeTo(Self));
        }

        protected override void RecoverFromSnapshot()
        {
            RegisterAggregatePersistence();
        }

        protected virtual void AwaitingCommandBehavior()
        {
            DefaultBehavior();
            Command<NotifyOnCommandComplete>(n =>
                                             {
                                                 _commandCompletedWaiters.Add(Sender);
                                                 Sender.Tell(NotifyOnCommandCompletedAck.Instance);
                                             });

            Command<IMessageMetadataEnvelop<ICommand>>(m =>
                                                       {
                                                           var cmd = m.Message;
                                                           Monitor.IncrementMessagesReceived();
                                                           Log.Debug("{Aggregate} received a {@command}", PersistenceId, cmd);

                                                           _aggregateCommandsHandler.ExecuteAsync(State, cmd)
                                                                                    .PipeTo(Self);

                                                           Behavior.Become(() => ProcessingCommandBehavior(m), nameof(ProcessingCommandBehavior));
                                                       });
        }

        private void ProcessingCommandBehavior(IMessageMetadataEnvelop<ICommand> commandEnvelop)
        {
            var command = commandEnvelop.Message;
            var commandMetadata = commandEnvelop.Metadata;
            var producedEventsMetadata = commandMetadata.CreateChild(Id, _domainEventProcessEntry);
            //finished some call on aggregate, need persist produced events
            Command<SaveEventsAsync>(e =>
                                     {
                                         var domainEvents = e.NewState.GetDomainEvents();
                                         if (!domainEvents.Any())
                                         {
                                             Log.Warning("Aggregate {id} is saving zero events", PersistenceId);
                                         }

                                         PersistAll(domainEvents.Select(evt => evt.CloneWithSaga(command.SagaId)),
                                                    persistedEvent =>
                                                    {
                                                        try
                                                        {
                                                            e.NewState.MarkPersisted(persistedEvent);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Log.Error(ErrorOnEventApplyText, Id, command);
                                                            PublishError(command, commandMetadata, ex);
                                                            
                                                            //intentionally drop all pending commands and messages
                                                            //and processing projection builders as
                                                            //state is corrupted
                                                            Context.Stop(Self);
                                                            return;
                                                        }

                                                        NotifyPersistenceWatchers(persistedEvent);
                                                        TrySaveSnapshot(e.NewState);
                                                        Project(persistedEvent, producedEventsMetadata).PipeTo(Self);

                                                        if (e.NewState.HasUncommitedEvents)
                                                            return;

                                                        try
                                                        {
                                                            e.Continuation();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Log.Error(ErrorOnContinuationText, Id, command);

                                                            var self = Self;
                                                            PublishError(command, commandMetadata, ex).ContinueWith(t => self.Tell(e.NewState));
                                                            return;
                                                        }

                                                        Self.Tell(e.NewState);
                                                    });
                                     });
            //aggregate raised an error during command execution
            Command<Status.Failure>(f => PublishError(command, commandMetadata, f.Cause).PipeTo(Self));

            //aggregate command execution is finished 
            //produced events are persisted
            //we can have events not projected yet
            Command<TAggregate>(newState =>
                                {
                                    //special case
                                    //aggregate was just created, only constructor was called
                                    //need persist its events
                                    if (!ReferenceEquals(newState, State))
                                    {
                                        //renew state to not fall into recursion
                                        Self.Tell(new SaveEventsAsync(newState, () =>
                                                                                {
                                                                                    State = newState;
                                                                                    //attach persistence to newly created aggregate
                                                                                    RegisterAggregatePersistence();
                                                                                }));
                                        return;
                                    }

                                    //handler finished execution but need to wait for events persistence
                                    //or aggregate method started execution but don't produce anything yet
                                    //or projection in progress, will finish execution later by notification from message processor
                                    if (IsProjecting || newState.HasUncommitedEvents || newState.IsMethodExecuting)
                                        return;

                                
                                    CompleteCommandExecution(command);
                                });
            //projection of event pack from aggregate finished
            //we can have more event packs to project
            Command<AllHandlersCompleted>(m =>
                                          {
                                              //publish messages for notification
                                              object projected;
                                              if (!_messagesToProject.TryGetValue(m.ProjectId, out projected))
                                                  throw new UnknownProjectionFinishedException();

                                              _messagesToProject.Remove(m.ProjectId);

                                              _publisher.Publish(projected, producedEventsMetadata);

                                              if (IsProjecting || State.HasUncommitedEvents)
                                                  return;

                                              if (_shouldTerminate)
                                              {
                                                  Self.Tell(GracefullShutdownRequest.Instance);
                                                  _shouldTerminate = true;
                                              }

                                              CompleteCommandExecution(command);
                                          });

            Command<IMessageMetadataEnvelop<ICommand>>(o => StashMessage(o));

            Command<GracefullShutdownRequest>(o => StashMessage(o));

            DefaultBehavior();
        }

        private Task<AllHandlersCompleted> PublishError(ICommand command, IMessageMetadata commandMetadata, Exception exception)
        {
            var producedFaultMetadata = commandMetadata.CreateChild(command.Id, _domainEventProcessFailEntry);

            var fault = Fault.NewGeneric(command, exception, command.SagaId, typeof(TAggregate));
            Log.Error(exception, "{Aggregate} raised an error {@Exception} while executing {@Command}", PersistenceId, exception, command);

            return Project(fault, producedFaultMetadata).ContinueWith(t =>
                                                                      {
                                                                          _publisher.Publish(fault, producedFaultMetadata);
                                                                          return t.Result;
                                                                      });
        }

        private void CompleteCommandExecution(ICommand cmd)
        {
            Behavior.Unbecome();
            Stash.UnstashAll();
            base.State.ClearUncommittedEvents();
            foreach (var waiter in _commandCompletedWaiters)
                waiter.Tell(new CommandCompleted(cmd.Id));
        }

        protected override void TerminatingBehavior()
        {
            Command<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           Self.Tell(CancelShutdownRequest.Instance);
                                                           StashMessage(c);
                                                       });
            base.TerminatingBehavior();
        }

        private Task<AllHandlersCompleted> Project(object evt, IMessageMetadata commandMetadata)
        {
            var envelop = new MessageMetadataEnvelop<Project>(new Project(evt), commandMetadata);
            
            _messagesToProject.Add(envelop.Message.ProjectId, evt);

            switch (evt)
            {
                case FutureEventScheduledEvent e:
                    Handle(e, commandMetadata);
                    break;
                case FutureEventCanceledEvent e:
                    Handle(e, commandMetadata);
                    break;
            }

            return _customHandlersActor.Ask<AllHandlersCompleted>(envelop);
        }

        private Task Handle(FutureEventScheduledEvent futureEventScheduledEvent, IMessageMetadata messageMetadata)
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
            var scheduleEvent = new ScheduleCommand(command,
                                                    scheduleKey,
                                                    ExecutionOptions.ForCommand(message.RaiseTime,
                                                                                message.Event.GetType()),
                                                    metadata);

            return _schedulerActorRef.Ask<Scheduled>(scheduleEvent);
        }

        public static ScheduleKey CreateScheduleKey(Guid scheduleId, Guid aggregateId, string description)
        {
            return new ScheduleKey(scheduleId,
                                   $"{typeof(TAggregate).Name}_{aggregateId}_future_event_{scheduleId}",
                                   $"{typeof(TAggregate).Name}_futureEvents",
                                   "");
        }

        private Task Handle(FutureEventCanceledEvent futureEventCanceledEvent, IMessageMetadata metadata)
        {
            var message = futureEventCanceledEvent;
            var key = CreateScheduleKey(message.FutureEventId, message.SourceId, "");
            var unscheduleMessage = new Unschedule(key);
            return _schedulerActorRef.Ask<Unscheduled>(unscheduleMessage);
        }
    }
}