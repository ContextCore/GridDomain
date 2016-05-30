using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace ClusterSqlPersistenceIssue
{

        class SqlJournalNotPersistentActor : UntypedActor
        {
            List<string> _events = new List<string>();
            private readonly IActorRef _notifyActor;

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
                    _notifyActor.Tell(new Persisted() { Payload = m.Payload });
                }
            }
        }
}

