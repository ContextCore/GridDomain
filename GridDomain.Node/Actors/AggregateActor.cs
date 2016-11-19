using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Monitoring;
using Akka.Monitoring.Impl;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;
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
        private readonly TypedMessageActor<Unschedule> _unscheduleActorRef;
        private readonly List<IActorRef> _persistenceWaiters = new List<IActorRef>();
        public readonly Guid Id;
        private readonly SnapshotsSavePolicy _snapshotsPolicy;
        public IAggregate Aggregate { get; private set; }
        public override string PersistenceId { get; }

        private readonly ActorMonitor _monitor;
        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IConstructAggregates _aggregateConstructor;

        public AggregateActor(IAggregateCommandsHandler<TAggregate> handler,
                              TypedMessageActor<ScheduleCommand> schedulerActorRef,
                              TypedMessageActor<Unschedule> unscheduleActorRef,
                              IPublisher publisher,
                              SnapshotsSavePolicy snapshotsSavePolicy,
                              IConstructAggregates aggregateConstructor)
        {
            _aggregateConstructor = aggregateConstructor;
            _schedulerActorRef = schedulerActorRef;
            _unscheduleActorRef = unscheduleActorRef;
            _handler = handler;
            _publisher = publisher;
            PersistenceId = Self.Path.Name;
            Id = AggregateActorName.Parse<TAggregate>(Self.Path.Name).Id;
            Aggregate = aggregateConstructor.Build(typeof(TAggregate), Id, null);
            _monitor = new ActorMonitor(Context,typeof(TAggregate).Name);
            _snapshotsPolicy = snapshotsSavePolicy;

            //async aggregate method execution finished, aggregate already raised events
            //need process it in usual way
            Command<AsyncEventsReceived>(m =>
            {
                _monitor.IncrementMessagesReceived();
                if (m.Exception != null)
                {
                   _publisher.Publish(Fault.NewGeneric(m.Command, m.Exception, typeof(TAggregate),m.Command.SagaId));
                    return;
                }

                (Aggregate as Aggregate).FinishAsyncExecution(m.InvocationId);
                ProcessAggregateEvents(m.Command);
            });

            Command<GracefullShutdownRequest>(req =>
            {
                _monitor.IncrementMessagesReceived();
                Shutdown();
            });
            
            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));
            Command<SaveSnapshotSuccess>(s =>
            {
                NotifyWatchers(s);
                _snapshotsPolicy.SnapshotWasSaved(s.Metadata);
            });

            Command<NotifyOnPersistenceEvents>(c =>
            {
                var waiter = c.Waiter ?? Sender;
                if (IsRecoveryFinished)
                {
                    waiter.Tell(RecoveryCompleted.Instance);
                }
                else _persistenceWaiters.Add(waiter);
            });
           
            Command<ICommand>(cmd =>
            {
                _monitor.IncrementMessagesReceived();
                _log.Trace("{Aggregate} received a {@command}", Aggregate.Id, cmd);
                try
                {
                    Aggregate = _handler.Execute((TAggregate)Aggregate, cmd);
                }
                catch (Exception ex)
                {
                    _publisher.Publish(Fault.NewGeneric(cmd, ex, typeof(TAggregate),cmd.SagaId));
                    Log.Error(ex,"{Aggregate} raised an expection {@Exception} while executing {@Command}",Aggregate.Id,ex,cmd);
                    return;
                }

                ProcessAggregateEvents(cmd);
            });

            Recover<SnapshotOffer>(offer =>
            {
                Aggregate = _aggregateConstructor.Build(typeof(TAggregate), Id, (IMemento)offer.Snapshot);
                _snapshotsPolicy.SnapshotWasApplied(offer.Metadata);
            });
            Recover<DomainEvent>(e =>
            {
                Aggregate.ApplyEvent(e);
                _snapshotsPolicy.RefreshActivity(e.CreatedTime);
            });
            Recover<RecoveryCompleted>(c =>
             {
                Log.Debug("Recovery for actor {Id} is completed", PersistenceId);
                 //notify all 
                 NotifyWatchers(c);
               // _persistenceWaiters.Clear();
            });
            

        }

        private void NotifyWatchers(object msg)
        {
            foreach (var watcher in _persistenceWaiters)
                watcher.Tell(msg);
        }

        protected virtual void Shutdown()
        {
            DeleteSnapshots(_snapshotsPolicy.SnapshotsToDelete());
            Become(Terminating);
        }

        void Terminating()
        {
            Command<DeleteSnapshotsSuccess>(s =>
            {
                NotifyWatchers(s);
                Context.Stop(Self);
            });
            Command<DeleteSnapshotsFailure>(s =>
            {
                NotifyWatchers(s);
                Context.Stop(Self);
            });
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            Log.Error("Additional persistence diagnostics on fauilure {error} {actor} {event}", cause, Self.Path.Name, @event);
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            Log.Error("Additional persistence diagnostics on rejected {error} {actor} {event}", cause, Self.Path.Name, @event);
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        private void ProcessAggregateEvents(ICommand command)
        {
            var events = Aggregate.GetUncommittedEvents().Cast<DomainEvent>().ToArray();

            if (command.SagaId != Guid.Empty)
            {
                events = events.Select(e => e.CloneWithSaga(command.SagaId)).ToArray();
            }

            PersistAll(events, e =>
            {
                //TODO: move scheduling event processing to some separate handler or aggregateActor extension. 
                //how to pass aggregate type in this case? 
                //direct call to method to not postpone process of event scheduling, 
                //case it can be interrupted by other messages in stash processing errors
                e.Match().With<FutureEventScheduledEvent>(Handle)
                         .With<FutureEventCanceledEvent>(Handle);

                _publisher.Publish(e);
            });



            Aggregate.ClearUncommittedEvents();

            ProcessAsyncMethods(command);

            if(_snapshotsPolicy.ShouldSave(events))
                SaveSnapshot(Aggregate.GetSnapshot());
        }
        
        private void ProcessAsyncMethods(ICommand command)
        {
            var extendedAggregate = Aggregate as Aggregate;
            if (extendedAggregate == null) return;

            //When aggregate notifies external world about async method execution start,
            //actor should schedule results to process it
            //command is included to safe access later, after async execution complete
            var cmd = command;
            foreach (var asyncMethod in extendedAggregate.GetAsyncUncomittedEvents())
                asyncMethod.ResultProducer.ContinueWith(t => new AsyncEventsReceived(t.IsFaulted ? null: t.Result, cmd, asyncMethod.InvocationId, t.Exception))
                                          .PipeTo(Self);

            extendedAggregate.ClearAsyncUncomittedEvents();
        }

        public void Handle(FutureEventScheduledEvent message)
        {
            Guid scheduleId = message.Id;
            Guid aggregateId = message.Event.SourceId;

            var description = $"Aggregate {typeof(TAggregate).Name} id = {aggregateId} scheduled future event " +
                              $"{scheduleId} with payload type {message.Event.GetType().Name} on time {message.RaiseTime}\r\n" +
                              $"Future event: {message.ToPropsString()}";

            var scheduleKey = CreateScheduleKey(scheduleId, aggregateId, description);

            var scheduleEvent = new ScheduleCommand(new RaiseScheduledDomainEventCommand(message.Id, message.SourceId, Guid.NewGuid()),
                                                    scheduleKey,
                                                    new ExecutionOptions(message.RaiseTime, message.Event.GetType()));

            _schedulerActorRef.Handle(scheduleEvent);
        }

        public static ScheduleKey CreateScheduleKey(Guid scheduleId, Guid aggregateId, string description)
        {
            return new ScheduleKey(scheduleId,
                                   $"{typeof(TAggregate).Name}_{aggregateId}_future_event_{scheduleId}",
                                   $"{typeof(TAggregate).Name}_futureEvents",
                                   "");
        }

        public void Handle(FutureEventCanceledEvent message)
        {
            var key = CreateScheduleKey(message.FutureEventId, message.SourceId, "");
            var unscheduleMessage = new Unschedule(key);
            _unscheduleActorRef.Handle(unscheduleMessage);
        }


        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
        }
        protected override void PreRestart(Exception reason, object message)
        {
            _monitor.IncrementActorRestarted();
        }
    }
}