using Akka.Actor;

namespace GridDomain.Node.Actors.Sagas.Messages
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