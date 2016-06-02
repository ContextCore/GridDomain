using System.Collections.Generic;
using Akka.Actor;

namespace GridDomain.Tests.Acceptance.Persistence
{
    internal class SqlJournalNotPersistentActor : UntypedActor
    {
        private readonly IActorRef _notifyActor;
        private List<string> _events = new List<string>();

        public SqlJournalNotPersistentActor(IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
            var plugin = Akka.Persistence.Persistence.Instance.Apply(Context.System).JournalFor(null);
            plugin.Tell(new object());
        }

        protected override void OnReceive(object message)
        {
            if (message is SqlJournalPing)
            {
                var m = message as SqlJournalPing;
                //  _events.Add(m.Payload);
                _notifyActor.Tell(new Persisted {Payload = m.Payload});
            }
        }
    }
}