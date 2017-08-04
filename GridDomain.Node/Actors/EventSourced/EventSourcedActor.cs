using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.AkkaMessaging;
using SubscribeAck = GridDomain.Node.Transports.Remote.SubscribeAck;

namespace GridDomain.Node.Actors.EventSourced
{
    public class DomainEventSourcedActor<T> : ReceivePersistentActor where T : Aggregate
    {
        private readonly List<IActorRef> _persistenceWatchers = new List<IActorRef>();
        private readonly ISnapshotsPersistencePolicy _snapshotsPolicy;
        protected readonly ActorMonitor Monitor;

        protected readonly BehaviorStack Behavior;

        public DomainEventSourcedActor(IConstructAggregates aggregateConstructor, ISnapshotsPersistencePolicy policy)
        {
            _snapshotsPolicy = policy;

            PersistenceId = Self.Path.Name;
            Id = AggregateActorName.Parse<T>(Self.Path.Name).
                                    Id;
            State = (T) aggregateConstructor.Build(typeof(T), Id, null);

            Monitor = new ActorMonitor(Context, typeof(T).Name);
            Behavior = new BehaviorStack(BecomeStacked, UnbecomeStacked);

            DefaultBehavior();

            Recover<DomainEvent>(e => { ((IAggregate) State).ApplyEvent(e); });

            Recover<SnapshotOffer>(offer =>
                                   {
                                       _snapshotsPolicy.MarkSnapshotApplied(offer.Metadata.SequenceNr);
                                       State = (T) aggregateConstructor.Build(typeof(T), Id, (IMemento) offer.Snapshot);
                                       RecoverFromSnapshot();
                                   });

            Recover<RecoveryCompleted>(message =>
                                       {
                                           Log.Debug("Recovery for actor {Id} is completed", PersistenceId);
                                           NotifyPersistenceWatchers(message);
                                       });
        }

        protected virtual void RecoverFromSnapshot() { }

        protected void DefaultBehavior()
        {
            Command<GracefullShutdownRequest>(req =>
                                              {
                                                  Log.Debug("{Actor} received shutdown request", PersistenceId);
                                                  Monitor.IncrementMessagesReceived();
                                                  Behavior.Become(TerminatingBehavior, nameof(TerminatingBehavior));
                                                  Self.Tell(req);
                                              });

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Command<NotifyOnPersistenceEvents>(c => SubscribePersistentObserver(c));

            Command<SaveSnapshotSuccess>(s =>
                                         {
                                             SnapshotsSaveInProgressCount--;
                                             NotifyPersistenceWatchers(s);
                                             _snapshotsPolicy.MarkSnapshotSaved(s.Metadata.SequenceNr,
                                                                                BusinessDateTime.UtcNow);
                                         });
        }

        protected void StashMessage(object message)
        {
            Log.Debug("Aggregate {id} stashing message {message} current behavior is {behavior}", PersistenceId, message, Behavior.Current);

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

        protected Guid Id { get; }
        public override string PersistenceId { get; }
        public T State { get; protected set; }
        protected int SnapshotsSaveInProgressCount;

        protected void SaveSnapshot(IAggregate aggregate)
        {
            if (_snapshotsPolicy.ShouldSave(
                                         SnapshotSequenceNr,
                                         BusinessDateTime.UtcNow))
            {
                SnapshotsSaveInProgressCount++;
                SaveSnapshot(aggregate.GetSnapshot());
            }
        }

        protected void NotifyPersistenceWatchers(object msg)
        {
            foreach (var watcher in _persistenceWatchers)
                watcher.Tell(new Persisted(msg));
        }


        protected virtual void TerminatingBehavior()
        {
            Command<DeleteSnapshotsSuccess>(s =>
                                            {
                                                Log.Debug("snapshots deleted, {criteria}", s.Criteria);
                                                StopNow();
                                            });
            Command<DeleteSnapshotsFailure>(s =>
                                            {
                                                Log.Debug("snapshots failed to delete, {criteria}", s.Criteria);
                                                StopNow();
                                            });
            //for cases when actor is ask to termite and snapshot save is in progress
            Command<SaveSnapshotSuccess>(s =>
                                         {
                                             SnapshotsSaveInProgressCount--;
                                             if (SnapshotsSaveInProgressCount != 0)
                                                 return;

                                             Log.Debug("All snapshots blocking terminations were saved, continue work");
                                             Stash.UnstashAll();
                                         });

            Command<CancelShutdownRequest>(s =>
                                           {
                                               Behavior.Unbecome();
                                               Stash.UnstashAll();
                                               Log.Info("Shutdown canceled, will resume activity");
                                               Sender.Tell(ShutdownCanceled.Instance);
                                           });

            Command<GracefullShutdownRequest>(s =>
                                              {
                                                  var messageToProcess = Stash.ClearStash().
                                                                               Where(m => !(m.Message is GracefullShutdownRequest) ||
                                                                                          !(m.Message is DeleteSnapshotsSuccess) ||
                                                                                          !(m.Message is SaveSnapshotSuccess) ||
                                                                                          !(m.Message is DeleteSnapshotsFailure)).
                                                                               ToArray();

                                                  if (messageToProcess.Any())
                                                  {
                                                      Log.Warning("{Actor} received shutdown request but have unprocessed messages."
                                                                  + "Shutdown will be postponed until all messages processing",
                                                                  PersistenceId);

                                                      Behavior.Unbecome();
                                                      foreach (var m in messageToProcess)
                                                          Self.Tell(m.Message);

                                                      Self.Tell(s);
                                                      return;
                                                  }

                                                  if (SnapshotsSaveInProgressCount > 0)
                                                  {
                                                      Log.Debug("Snapshots save is in progress, will wait for it");
                                                      Stash.Stash();
                                                      return;
                                                  }

                                                  if (!_snapshotsPolicy.ShouldDelete(out GridDomain.Configuration.SnapshotSelectionCriteria c))
                                                      return;

                                                  var snapshotSelectionCriteria = new Akka.Persistence.SnapshotSelectionCriteria(c.MaxSequenceNr, c.MaxTimeStamp, c.MinSequenceNr, c.MinTimestamp);
                                                  DeleteSnapshots(snapshotSelectionCriteria);
                                                  Log.Debug("started snapshots delete, {criteria}", snapshotSelectionCriteria);
                                                  StopNow();
                                              });
        }

        protected override void Unhandled(object message)
        {
            Log.Warning("Actor {id} skipping message {message} because it was unhandled. \r\n {@behavior}.",
                        PersistenceId,
                        message,
                        Behavior);
            base.Unhandled(message);
        }

        private void StopNow()
        {
            Log.Debug("Event sourced actor {id} stopped", PersistenceId);
            Context.Stop(Self);
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