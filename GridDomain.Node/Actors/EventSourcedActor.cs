using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class EventSourcedActor<T> : ReceivePersistentActor where T: AggregateBase
    {
        private readonly List<IActorRef> _persistenceWatchers = new List<IActorRef>();
        protected Guid Id { get; }
        protected readonly ISnapshotsPersistencePolicy SnapshotsPolicy;
        protected readonly ActorMonitor Monitor;
        protected ILoggingAdapter _log = Context.GetLogger();

        private readonly IConstructAggregates _aggregateConstructor;
        public override string PersistenceId { get; }
        public IAggregate State { get; protected set; }

        public EventSourcedActor(IConstructAggregates aggregateConstructor,
                                 ISnapshotsPersistencePolicy policy)
        {
            PersistenceId = Self.Path.Name;
            SnapshotsPolicy = policy;
            _aggregateConstructor = aggregateConstructor;
            Id = AggregateActorName.Parse<T>(Self.Path.Name).Id;
            State = (AggregateBase)aggregateConstructor.Build(typeof(T), Id, null);
            Monitor = new ActorMonitor(Context, typeof(T).Name);

            Command<GracefullShutdownRequest>(req =>
            {
                Monitor.IncrementMessagesReceived();
                DeleteSnapshots(SnapshotsPolicy.GetSnapshotsToDelete());
                Become(Terminating);
            });

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Command<SaveSnapshotSuccess>(s =>
            {
                NotifyPersistenceWatchers(s);
                SnapshotsPolicy.MarkSnapshotSaved(s.Metadata);
            });

            Command<NotifyOnPersistenceEvents>(c =>
            {
                var waiter = c.Waiter ?? Sender;
                if (IsRecoveryFinished)
                    waiter.Tell(RecoveryCompleted.Instance);

                 _persistenceWatchers.Add(waiter);
            });

            Recover<DomainEvent>(e =>
            {
                State.ApplyEvent(e);
                SnapshotsPolicy.MarkActivity(e.CreatedTime);
            });

            Recover<SnapshotOffer>(offer =>
            {
                SnapshotsPolicy.MarkSnapshotApplied(offer.Metadata);
                State = _aggregateConstructor.Build(typeof(T), Id, (IMemento)offer.Snapshot);
            });

            Recover<RecoveryCompleted>(message =>
            {
                _log.Debug("Recovery for actor {Id} is completed", PersistenceId);
                NotifyPersistenceWatchers(message);
            });
        }

        protected bool TrySaveSnapshot(params object[] stateChange)
        {
            var shouldSave = SnapshotsPolicy.ShouldSave(stateChange);
            if (shouldSave)
                SaveSnapshot(State.GetSnapshot());
            return shouldSave;
        }

        protected void NotifyPersistenceWatchers(object msg)
        {
            foreach (var watcher in _persistenceWatchers)
                watcher.Tell(new Persisted(msg));
        }

        private void Terminating()
        {
            //for case when we in process of saving snapshot or events
            Command<DeleteSnapshotsSuccess>(s => DeferAsync(new object(), o => StopNow(s)));
            Command<DeleteSnapshotsFailure>(s => DeferAsync(new object(), o => StopNow(s)));
        }

        private void StopNow(object s)
        {
            NotifyPersistenceWatchers(s);
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