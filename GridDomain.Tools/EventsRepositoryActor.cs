using System;
using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Persistence;

namespace GridDomain.Tools
{
    public class EventsRepositoryActor : ReceivePersistentActor
    {
        private readonly List<object> _events = new List<object>();
        public override string PersistenceId { get; }

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

            Command<Persist>(m => Persist(m.Msg, e => Sender.Tell(new Persisted(m), Self)));
            Command<Load>(m => Sender.Tell(new Loaded(id, _events.ToArray())));
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