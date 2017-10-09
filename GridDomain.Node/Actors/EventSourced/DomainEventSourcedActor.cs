using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.AkkaMessaging;
using SubscribeAck = GridDomain.Transport.Remote.SubscribeAck;

namespace GridDomain.Node.Actors.EventSourced
{
    public class DomainEventSourcedActor<T> : ReceivePersistentActor where T : IAggregate
    {
        private readonly List<IActorRef> _persistenceWatchers = new List<IActorRef>();
        private readonly ISnapshotsPersistencePolicy _snapshotsPolicy;
        protected readonly ActorMonitor Monitor;

        protected readonly BehaviorStack Behavior;
        protected override ILoggingAdapter Log { get; } = Context.GetLogger(new SerilogLogMessageFormatter());

        public DomainEventSourcedActor(IConstructAggregates aggregateConstructor, ISnapshotsPersistencePolicy policy)
        {
            _snapshotsPolicy = policy;
          
            PersistenceId = Self.Path.Name;
            Id = AggregateActorName.Parse<T>(Self.Path.Name)
                                   .Id;
            State = (T) aggregateConstructor.Build(typeof(T), Id, null);

            Monitor = new ActorMonitor(Context, typeof(T).Name);
            Behavior = new BehaviorStack(BecomeStacked, UnbecomeStacked);

            DefaultBehavior();

            Recover<DomainEvent>(e => { State.ApplyEvent(e); });

            Recover<SnapshotOffer>(offer =>
                                   {
                                       _snapshotsPolicy.MarkSnapshotApplied(offer.Metadata.SequenceNr);
                                       State = (T) aggregateConstructor.Build(typeof(T), Id, (IMemento) offer.Snapshot);
                                       Log.Debug("Built state from snapshot #{snapshotNum}", offer.Metadata.SequenceNr);
                                   });

            Recover<RecoveryCompleted>(message =>
                                       {
                                           Log.Debug("Recovery completed");
                                           NotifyPersistenceWatchers(message);
                                       });
        }

        protected void DefaultBehavior()
        {
            Command<GracefullShutdownRequest>(req =>
                                              {
                                                  Log.Debug("Received shutdown request");
                                                  Monitor.IncrementMessagesReceived();
                                                  Behavior.Become(TerminatingBehavior, nameof(TerminatingBehavior));
                                                  Self.Tell(req);
                                              });

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Command<NotifyOnPersistenceEvents>(c => SubscribePersistentObserver(c));

            Command<SaveSnapshotSuccess>(s =>
                                         {
                                             CountSnapshotSaved(s);
                                         });

            Command<DeleteSnapshotSuccess>(s =>
                                           {
                                               Log.Debug("snapshot deleted");
                                               NotifyPersistenceWatchers(s);
                                           });
        }

        protected void StashMessage(object message)
        {
            Log.Debug("Stashing message {message} current behavior is {behavior}", message, Behavior.Current);

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

        protected void SaveSnapshot(IAggregate aggregate, object lastEventPersisted)
        {
            if (!_snapshotsPolicy.ShouldSave(SnapshotSequenceNr, BusinessDateTime.UtcNow)) return;
            Log.Debug("Started snapshot save, cased by persisted event {event}",lastEventPersisted);
            _snapshotsPolicy.MarkSnapshotSaving();
            SaveSnapshot(aggregate.GetSnapshot());
        }

        protected void NotifyPersistenceWatchers(object msg)
        {
            foreach (var watcher in _persistenceWatchers)
                watcher.Tell(msg);
        }

        protected virtual void TerminatingBehavior()
        {
            Command<DeleteSnapshotsSuccess>(s =>
                                            {
                                                Log.Debug("snapshots deleted, {criteria}", s.Criteria);
                                                NotifyPersistenceWatchers(s);
                                                StopNow();
                                            });
            Command<DeleteSnapshotsFailure>(s =>
                                            {
                                                Log.Debug("snapshots failed to delete, {criteria}", s.Criteria);
                                                NotifyPersistenceWatchers(s);
                                                StopNow();
                                            });
            //for cases when actor is ask to termite and snapshot save is in progress
            Command<SaveSnapshotSuccess>(s =>
                                         {
                                             Log.Debug("snapshot saved during termination");
                                             CountSnapshotSaved(s);

                                             if (_snapshotsPolicy.SnapshotsSaveInProgress) return;
                                             
                                             Log.Debug("All snapshots blocking terminations were saved, continue work");
                                             Stash.UnstashAll();
                                         });

            Command<GracefullShutdownRequest>(s =>
                                              {
                                                  Log.Debug("Started gracefull shutdown process");
                                                  var messageToProcess = Stash.ClearStash()
                                                                              .Where(m => !(m.Message is GracefullShutdownRequest)) 
                                                                              .ToArray();

                                                  if (messageToProcess.Any())
                                                  {
                                                      Log.Warning("Received shutdown request but have unprocessed messages."
                                                                  + "Shutdown will be postponed until all messages processing");

                                                      Behavior.Unbecome();
                                                      foreach (var m in messageToProcess)
                                                          Self.Tell(m.Message);

                                                      Self.Tell(s);
                                                      return;
                                                  }

                                                  if (_snapshotsPolicy.SnapshotsSaveInProgress)
                                                  {
                                                      Log.Debug("Snapshots save is in progress, will wait for it");
                                                      Stash.Stash();
                                                      return;
                                                  }

                                                  if (_snapshotsPolicy.ShouldDelete(out GridDomain.Configuration.SnapshotSelectionCriteria c))
                                                  {
                                                      var snapshotSelectionCriteria = new Akka.Persistence.SnapshotSelectionCriteria(c.MaxSequenceNr, c.MaxTimeStamp, c.MinSequenceNr, c.MinTimestamp);
                                                      Log.Debug("started snapshots delete, {criteria}", snapshotSelectionCriteria);
                                                      DeleteSnapshots(snapshotSelectionCriteria);
                                                      return;
                                                  }

                                                  StopNow();
                                              });
        }

        private void CountSnapshotSaved(SaveSnapshotSuccess s)
        {
            Log.Debug("snapshot saved at {time}, sequence number is {number}", s.Metadata.Timestamp,s.Metadata.SequenceNr);
            NotifyPersistenceWatchers(s);
            _snapshotsPolicy.MarkSnapshotSaved(s.Metadata.SequenceNr,
                                               BusinessDateTime.UtcNow);
        }

        protected override void Unhandled(object message)
        {
            Log.Warning("Skipping message {message} because it was unhandled. \r\n Behavior: {@behavior}.",message,Behavior);
            base.Unhandled(message);
        }

        private void StopNow()
        {
            Log.Debug("Stopped");
            Context.Stop(Self);
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