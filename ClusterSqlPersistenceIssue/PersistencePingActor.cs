using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;

namespace ClusterSqlPersistenceIssue
{
    class PersistencePingActor : PersistentActor
    {
        List<string> _events = new List<string>();
        private readonly IActorRef _notifyActor;
        private readonly IActorRef _plugin;

        public PersistencePingActor(IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
// _plugin = Persistence.Instance.Apply(Context.System).JournalFor(null);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                _events = (List<string>)((SnapshotOffer)message).Snapshot;
            }
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is SqlJournalPing)
            {
                var m = message as SqlJournalPing;
                _events.Add(m.Payload);
                Persist(m.Payload, e => _notifyActor.Tell(new Persisted() { Payload = m.Payload, JournalActorName = "1232"}));
            }
            return true;
        }

        public override string PersistenceId => "test";
    }
}