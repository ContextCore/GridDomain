using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Persistence;

namespace GridDomain.Tools.Repositories
{
    public class EventsRepositoryActor : ReceivePersistentActor
    {
        private readonly List<object> _events = new List<object>();

        public EventsRepositoryActor(string id)
        {
            PersistenceId = id;

            RecoverAny(m =>
                       {
                           m.Match()
                            .With<SnapshotOffer>(so => { })
                            .With<RecoveryCompleted>(f => { })
                            .Default(e => { _events.Add(e); });
                       });

            Command<Persist>(m =>
                             {
                                 var sender = Sender;
                                 Persist(m.Msg, e => sender.Tell(new Persisted(m.Msg), Self));
                             });

            Command<Load>(m => Sender.Tell(new Loaded(id, _events.ToArray())));
        }

        public override string PersistenceId { get; }

        public class Load {}

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
            public Loaded(string persistenceId, params object[] events)
            {
                PersistenceId = persistenceId;
                Events = events;
            }

            public string PersistenceId { get; }

            public object[] Events { get; }
        }

        public class Persisted
        {
            public Persisted(object payload)
            {
                Payload = payload;
            }

            public object Payload { get; }
        }
    }
}