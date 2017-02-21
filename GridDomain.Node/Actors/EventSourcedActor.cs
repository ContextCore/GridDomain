using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka.Remote;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class EventSourcedActor<T> : ReceivePersistentActor where T: AggregateBase
    {
        private readonly List<IActorRef> _persistenceWatchers = new List<IActorRef>();
        protected Guid Id { get; }
        private readonly ISnapshotsPersistencePolicy _snapshotsPolicy;
        protected readonly ActorMonitor Monitor;

        private readonly IConstructAggregates _aggregateConstructor;
        public override string PersistenceId { get; }
        public IAggregate State { get; protected set; }

        private int _terminateWaitCount = 3;
        private readonly TimeSpan _terminateWaitPeriod = TimeSpan.FromSeconds(1);


        public EventSourcedActor(IConstructAggregates aggregateConstructor,
                                 ISnapshotsPersistencePolicy policy)
        {
            _snapshotsPolicy = policy;
            _aggregateConstructor = aggregateConstructor;

            PersistenceId = Self.Path.Name;
            Id = AggregateActorName.Parse<T>(Self.Path.Name).Id;
            State = (AggregateBase)aggregateConstructor.Build(typeof(T), Id, null);
            Monitor = new ActorMonitor(Context, typeof(T).Name);

            Command<GracefullShutdownRequest>(req =>
            {
                Monitor.IncrementMessagesReceived();
                Become(Terminating);
            });

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Command<SaveSnapshotSuccess>(s =>
            {
                NotifyPersistenceWatchers(s);
                _snapshotsPolicy.MarkSnapshotSaved(s.Metadata.SequenceNr, BusinessDateTime.UtcNow);
            });

            Command<NotifyOnPersistenceEvents>(c =>
            {
                var waiter = c.Waiter ?? Sender;
                if (IsRecoveryFinished)
                    waiter.Tell(RecoveryCompleted.Instance);

                 _persistenceWatchers.Add(waiter);
                Sender.Tell(SubscribeAck.Instance);
            });

            Recover<DomainEvent>(e =>
            {
                State.ApplyEvent(e);
            });

            Recover<SnapshotOffer>(offer =>
            {
                _snapshotsPolicy.MarkSnapshotApplied(offer.Metadata.SequenceNr);
                State = _aggregateConstructor.Build(typeof(T), Id, (IMemento)offer.Snapshot);
            });

            Recover<RecoveryCompleted>(message =>
            {
                Log.Debug("Recovery for actor {Id} is completed", PersistenceId);
                NotifyPersistenceWatchers(message);
            });
        }

        protected bool TrySaveSnapshot()
        {
            return _snapshotsPolicy.TrySave(() => SaveSnapshot(State.GetSnapshot()), SnapshotSequenceNr, BusinessDateTime.UtcNow);
        }

        protected void NotifyPersistenceWatchers(object msg)
        {
            foreach (var watcher in _persistenceWatchers)
                watcher.Tell(new Persisted(msg));
        }

        private void Terminating()
        {
             
             //for case when we in process of saving snapshot or events
             Command<DeleteSnapshotsSuccess>(s => StopNow(s));
             Command<DeleteSnapshotsFailure>(s => StopNow(s));
             Command<GracefullShutdownRequest>(s =>
                                               {
                                                   if (_snapshotsPolicy.TryDelete(DeleteSnapshots))
                                                   {
                                                       Log.Debug("started snapshots delete");
                                                       return;
                                                   }   

                                                   Log.Debug("Unsaved snapshots found");
                                                   if (--_terminateWaitCount < 0) return;

                                                   Log.Debug("Will retry to delete snapshots in {time}, times left:{times}",
                                                       _terminateWaitPeriod,
                                                       _terminateWaitCount);

                                                   Context.System.Scheduler.ScheduleTellOnce(_terminateWaitPeriod,
                                                       Self,
                                                       GracefullShutdownRequest.Instance,
                                                       Self);
                                               }); 
            //start terminateion process
            Self.Tell(GracefullShutdownRequest.Instance);     
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