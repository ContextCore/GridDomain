using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class EventSourcedActor<T> : ReceivePersistentActor where T: IAggregate
    {
        protected readonly List<IActorRef> _persistenceWaiters = new List<IActorRef>();
        public Guid Id { get; }
        protected readonly SnapshotsSavePolicy _snapshotsPolicy;
        protected readonly ActorMonitor _monitor;
        protected readonly ISoloLogger _log = LogManager.GetLogger();
        protected readonly IPublisher _publisher;
        protected readonly IConstructAggregates _aggregateConstructor;
        public override string PersistenceId { get; }
        public IAggregate State { get; protected set; }


        public EventSourcedActor(IConstructAggregates aggregateConstructor,
                                 SnapshotsSavePolicy policy,
                                 IPublisher publisher)
        {
            PersistenceId = Self.Path.Name;
            _snapshotsPolicy = policy;
            _aggregateConstructor = aggregateConstructor;
            _publisher = publisher;
            Id = AggregateActorName.Parse<T>(Self.Path.Name).Id;
            State = aggregateConstructor.Build(typeof(T), Id, null);
            _monitor = new ActorMonitor(Context, typeof(T).Name);

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


            Recover<DomainEvent>(e =>
            {
                State.ApplyEvent(e);
                _snapshotsPolicy.RefreshActivity(e.CreatedTime);
            });

            Recover<SnapshotOffer>(offer =>
            {
                State = _aggregateConstructor.Build(typeof(T), Id, (IMemento)offer.Snapshot);
            });

            Recover<RecoveryCompleted>(message =>
            {
                Log.Debug("Recovery for actor {Id} is completed", PersistenceId);
                NotifyWatchers(message);
                //_persistenceWaiters.Clear();
            });
        }

        protected void NotifyWatchers(object msg)
        {
            foreach (var watcher in _persistenceWaiters)
                watcher.Tell(msg);
        }

        protected virtual void Shutdown()
        {
            DeleteSnapshots(_snapshotsPolicy.SnapshotsToDelete());
            Become(Terminating);
        }

        private void Terminating()
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