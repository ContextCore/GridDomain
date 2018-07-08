using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.AkkaMessaging;
using SnapshotSelectionCriteria = GridDomain.Configuration.SnapshotPolicies.SnapshotSelectionCriteria;
using SubscribeAck = GridDomain.Transport.Remote.SubscribeAck;

namespace GridDomain.Node.Actors.EventSourced
{
    public class DomainEventSourcedActor<T> : ReceivePersistentActor where T : class, IAggregate
    {
        private readonly List<IActorRef> _persistenceWatchers = new List<IActorRef>();
        private readonly ISnapshotsPersistencePolicy _snapshotsPolicy;
        private readonly IOperationTracker<long> _snapshotsSaveTracker;
        private readonly IOperationTracker<SnapshotSelectionCriteria> _snapshotsDeleteTracker;
        
        protected readonly ActorMonitor Monitor;

        protected readonly BehaviorQueue Behavior;
        private readonly IConstructSnapshots _snapshotsConstructor;
        protected override ILoggingAdapter Log { get; } = Context.GetSeriLogger();

        protected DomainEventSourcedActor(IConstructAggregates aggregateConstructor, 
                                          IConstructSnapshots snapshotsConstructor,
                                          ISnapshotsPersistencePolicy policy)
        {
            _snapshotsConstructor = snapshotsConstructor;
            
            _snapshotsPolicy = policy;
            _snapshotsSaveTracker = (policy as ISnapshotsSavePolicy).Tracking;
            _snapshotsDeleteTracker = (policy as ISnapshotsDeletePolicy).Tracking;
            
            PersistenceId = Self.Path.Name;
            Id = EntityActorName.Parse<T>(Self.Path.Name).Id;
            State = (T) aggregateConstructor.Build(typeof(T), Id, null);

            Monitor = new ActorMonitor(Context, typeof(T).Name);
            Behavior = new BehaviorQueue(Become);

            DefaultBehavior();

            Recover<DomainEvent>(e => State.Apply(e));

            Recover<SnapshotOffer>(offer =>
                                   {
                                       _snapshotsPolicy.MarkSnapshotApplied(offer.Metadata.SequenceNr);
                                       State = (T) aggregateConstructor.Build(typeof(T), Id, (IMemento) offer.Snapshot);
                                       Log.Debug("Built state from snapshot #{snapshotNum}", offer.Metadata.SequenceNr);
                                   });

            var recoveryTimer = Monitor.StartMeasureTime("Recovery");
            Recover<RecoveryCompleted>(message =>
                                       {
                                           recoveryTimer.Stop();
                                           Log.Debug("Recovery completed");
                                           NotifyPersistenceWatchers(message);
                                       });
        }

        protected virtual bool CanShutdown(out string description)
        {
            description = "";
            if (_snapshotsSaveTracker.InProgress != 0)
            {
                description += $"{_snapshotsSaveTracker.InProgress} snapshots saving is in progress ";
            } 
            if (_snapshotsDeleteTracker.InProgress != 0)
            {
                description += $"{_snapshotsDeleteTracker.InProgress} snapshots deleting is in progress";
            }

            return _snapshotsSaveTracker.InProgress == 0 && _snapshotsDeleteTracker.InProgress == 0;
        }
        protected void DefaultBehavior()
        {
            Command<Shutdown.Request>(req =>
                                              {
                                                  Monitor.IncrementMessagesReceived();

                                                  if (!CanShutdown(out var description))
                                                  {
                                                      Log.Debug("Shutdown request declined. Reason: \r\n {description}", description);
                                                      return;
                                                  }
                                                  
                                                  if (_snapshotsPolicy.ShouldDelete(out SnapshotSelectionCriteria c))
                                                  {
                                                      Log.Debug("Deleting snapnotExistshots");
                                                      DeleteSnapshots(c.ToGridDomain());
                                                      _snapshotsDeleteTracker.Start(c);
                                                      return;
                                                  }

                                                  Context.Stop(Self);
                                              });

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));
            Command<NotifyOnPersistenceEvents>(c => SubscribePersistentObserver(c));

           
            Command<DeleteSnapshotsSuccess>(s =>
                                            {
                                                _snapshotsDeleteTracker.Complete(s.Criteria.ToGridDomain());
                                                Log.Debug("Deleted snapshot bulk mode up to {reason}, in progress:{progress}",
                                                          s.Criteria, _snapshotsDeleteTracker.InProgress);
                                                NotifyPersistenceWatchers(s);
                                            });
            Command<DeleteSnapshotsFailure>(s =>
                                            {
                                                _snapshotsDeleteTracker.Fail(s.Criteria.ToGridDomain());
                                                Log.Debug("Failed to delete snapshots in bulk mode {reason}, delete in progress: {progress}", s,_snapshotsDeleteTracker.InProgress);
                                                NotifyPersistenceWatchers(s);
                                            });
            Command<SaveSnapshotSuccess>(s =>
                                         {
                                            
                                             NotifyPersistenceWatchers(s);
                                             Log.Debug("snapshot saved at {time}\r\n "
                                                       + "sequence number is {number} \r\n "
                                                       + "savings in progress: {progress} \r\n "
                                                       , s.Metadata.Timestamp,s.Metadata.SequenceNr, _snapshotsSaveTracker.InProgress);
                                            
                                             _snapshotsSaveTracker.Complete(s.Metadata.SequenceNr);
                                         });
            Command<SaveSnapshotFailure>(s =>
                                         {
                                            
                                             _snapshotsSaveTracker.Fail(s.Metadata.SequenceNr);
                                             NotifyPersistenceWatchers(s);
                                         });
        }

        protected void StashMessage(object message)
        {
            Log.Debug("Stashing message {@message} current behavior is {behavior}", message, Behavior.Current);

            Stash.Stash();
        }

        private void SubscribePersistentObserver(NotifyOnPersistenceEvents c)
        {
            var waiter = c.Waiter ?? Sender;
            if (IsRecoveryFinished)
                waiter.Tell(RecoveryCompleted.Instance);

            _persistenceWatchers.Add(waiter);
            Sender.Tell(SubscribeAck.Instance);
        }

        protected string Id { get; }
        public override string PersistenceId { get; }
        public T State { get; protected set; }

        protected void SaveSnapshot(IAggregate aggregate, object lastEventPersisted)
        {
            if (!_snapshotsPolicy.ShouldSave(SnapshotSequenceNr)) return;
            Log.Debug("Started snapshot save, cased by persisted event {event}",lastEventPersisted);
            SaveSnapshot(_snapshotsConstructor.GetSnapshot(aggregate));
            _snapshotsSaveTracker.Start(SnapshotSequenceNr);
        }

        protected void NotifyPersistenceWatchers(object msg)
        {
            foreach (var watcher in _persistenceWatchers)
                watcher.Tell(msg);
        }

        protected override void Unhandled(object message)
        {
            Log.Warning("Skipping message {message} because it was unhandled. \r\n Behavior: {@behavior}.",message,Behavior);
            base.Unhandled(message);
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            Log.Error("Additional persistence diagnostics on fauilure {error} {event}", cause, @event);
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            Log.Error("Additional persistence diagnostics on rejected {error} {event}", cause, @event);
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        protected override void PreStart()
        {
            Monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            Monitor.IncrementActorStopped();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Monitor.IncrementActorRestarted();
        }
    }
}