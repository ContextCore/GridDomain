using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class NotifyOnPersistenceEvents
    {
        public NotifyOnPersistenceEvents(IActorRef waiter)
        {
            Waiter = waiter;
        }

        public static readonly NotifyOnPersistenceEvents Instance = new NotifyOnPersistenceEvents(null);

        public IActorRef Waiter { get; }
    }
}