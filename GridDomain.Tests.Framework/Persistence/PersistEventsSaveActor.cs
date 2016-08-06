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
            Persist(message, e => Sender.Tell(new MessagePersisted(message),Self));
            return true;
        }

        public class MessagePersisted
        {
            public object Message { get;}

            public MessagePersisted(object message)
            {
                Message = message;
            }
        }

        public override string PersistenceId { get; }
    }
}