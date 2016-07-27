using Akka.Persistence;

namespace GridDomain.Tests.Framework.Persistence
{
    public class PersistEventsSaveActor : PersistentActor
    {
        public PersistEventsSaveActor(string persistenteId)
        {
            PersistenceId = persistenteId;
        }
        protected override bool ReceiveRecover(object message)
        {
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            PersistEvent(message);
            return true;
        }

        private void PersistEvent(object o)
        {
            Persist(o, e => { });
        }

        public override string PersistenceId { get; }
    }
}