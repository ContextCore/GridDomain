using Akka.Actor;

namespace GridDomain.Node.Actors
{
    internal class NotifyOnSagaTransited
    {
        public NotifyOnSagaTransited(IActorRef sender)
        {
            Sender = sender;
        }

        public IActorRef Sender { get; }
    }
}