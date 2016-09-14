using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class NotifyOnRecoverComplete
    {
        public NotifyOnRecoverComplete(IActorRef waiter)
        {
            Waiter = waiter;
        }
        public static readonly NotifyOnRecoverComplete Instance = new NotifyOnRecoverComplete(null);

        public IActorRef Waiter { get; }
    }
}