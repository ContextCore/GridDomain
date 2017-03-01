using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.Akka.Remote;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using Helios.Concurrency;

namespace GridDomain.Node.Actors
{
    public class EventSourcedActor<T> : ReceivePersistentActor where T : AggregateBase
    {
        private readonly IConstructAggregates _aggregateConstructor;
        private readonly List<IActorRef> _persistenceWatchers = new List<IActorRef>();
        private readonly ISnapshotsPersistencePolicy _snapshotsPolicy;
        private readonly TimeSpan _terminateWaitPeriod = TimeSpan.FromSeconds(1);
        protected readonly ActorMonitor Monitor;

        private int _terminateWaitCount = 3;

   

        public EventSourcedActor(IConstructAggregates aggregateConstructor, ISnapshotsPersistencePolicy policy)
        {
            _snapshotsPolicy = policy;
            _aggregateConstructor = aggregateConstructor;

            PersistenceId = Self.Path.Name;
            Id = AggregateActorName.Parse<T>(Self.Path.Name).Id;
            State = aggregateConstructor.Build(typeof(T), Id, null);

            ((Aggregate)State).RegisterPersistenceCallBack((e,act) => PersistAll(e, o =>
                                                                                    {
                                                                                        OnEventPersisted(e);
                                                                                        act(o);
                                                                                    }));



            Monitor = new ActorMonitor(Context, typeof(T).Name);

            Command<GracefullShutdownRequest>(req =>
                                              {
                                                  Log.Debug("{Actor} received shutdown request", PersistenceId);
                                                  Monitor.IncrementMessagesReceived();
                                                  BecomeStacked(Terminating);
                                              });

            Command<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Command<SaveSnapshotSuccess>(s =>
                                         {
                                             NotifyPersistenceWatchers(s);
                                             _snapshotsPolicy.MarkSnapshotSaved(s.Metadata.SequenceNr,
                                                                                BusinessDateTime.UtcNow);
                                         });

            Command<NotifyOnPersistenceEvents>(c =>
                                               {
                                                   var waiter = c.Waiter ?? Sender;
                                                   if (IsRecoveryFinished)
                                                       waiter.Tell(RecoveryCompleted.Instance);

                                                   _persistenceWatchers.Add(waiter);
                                                   Sender.Tell(SubscribeAck.Instance);
                                               });

            Recover<DomainEvent>(e => { State.ApplyEvent(e); });

            Recover<SnapshotOffer>(offer =>
                                   {
                                       _snapshotsPolicy.MarkSnapshotApplied(offer.Metadata.SequenceNr);
                                       State = _aggregateConstructor.Build(typeof(T), Id, (IMemento) offer.Snapshot);
                                   });

            Recover<RecoveryCompleted>(message =>
                                       {
                                           Log.Debug("Recovery for actor {Id} is completed", PersistenceId);
                                           NotifyPersistenceWatchers(message);
                                       });
        }

        protected Guid Id { get; }
        public override string PersistenceId { get; }
        public IAggregate State { get; protected set; }

        protected virtual void OnEventPersisted(DomainEvent[] events) {}

        protected bool TrySaveSnapshot()
        {
            return _snapshotsPolicy.TrySave(() => SaveSnapshot(State.GetSnapshot()),
                                            SnapshotSequenceNr,
                                            BusinessDateTime.UtcNow);
        }

        protected void NotifyPersistenceWatchers(object msg)
        {
            foreach (var watcher in _persistenceWatchers)
                watcher.Tell(new Persisted(msg));
        }

        protected virtual void Terminating()
        {
            //for case when we in process of saving snapshot or events
            Command<DeleteSnapshotsSuccess>(s => StopNow(s));
            Command<DeleteSnapshotsFailure>(s => StopNow(s));
            Command<CancelShutdownRequest>(s =>
                                           {
                                               UnbecomeStacked();
                                               Stash.UnstashAll();
                                               Log.Info("Aborting shutdown, will resume activity");
                                               Sender.Tell(ShutdownCanceled.Instance);
                                           });

            Command<GracefullShutdownRequest>(s =>
                                              {
                                                  if (_snapshotsPolicy.TryDelete(DeleteSnapshots))
                                                  {
                                                      Log.Debug("started snapshots delete");
                                                      return;
                                                  }

                                                  Log.Debug("Unsaved snapshots found");
                                                  if (--_terminateWaitCount < 0)
                                                      return;

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