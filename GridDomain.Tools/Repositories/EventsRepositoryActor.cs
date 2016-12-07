using System;
using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Logging;

namespace GridDomain.Tools.Repositories
{
    public class EventsRepositoryActor : ReceivePersistentActor
    {
        private readonly List<object> _events = new List<object>();
        public override string PersistenceId { get; }
        private ISoloLogger logger = LogManager.GetLogger();
        public EventsRepositoryActor(string id)
        {
            PersistenceId = id;

            RecoverAny(m =>
            {
                m.Match()
                 .With<SnapshotOffer>(so => { })
                 .With<RecoveryCompleted>(f => {})
                 .Default(e => _events.Add(e));
            });

            Command<Persist>(m =>Persist(m.Msg, e => Sender.Tell(new Persisted(m.Msg), Self)));
            Command<Load>(m => Sender.Tell(new Loaded(id, _events.ToArray())));
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        protected override void OnRecoveryFailure(Exception reason, object message = null)
        {
            base.OnRecoveryFailure(reason, message);
        }

        public class Load { }
        public class Persist
        {
            public readonly object Msg;

            public Persist(object msg)
            {
                Msg = msg;
            }
        }
        public class Loaded
        {
            public string PersistenceId { get; }

            public object[] Events { get; }

            public Loaded(string persistenceId, params object[] events)
            {
                this.PersistenceId = persistenceId;
                Events = events;
            }
        }
        public class Persisted
        {
            public object Payload { get; }

            public Persisted(object payload)
            {
                Payload = payload;
            }
        }
    }
}