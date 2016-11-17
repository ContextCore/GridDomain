using Akka.Actor;

namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class EventBusForwarder : ReceiveActor
    {
        private readonly IActorTransport _localTransport;

        public EventBusForwarder(IActorTransport localTransport)
        {
            _localTransport = localTransport;
            Receive<Publish>(p =>
            {
                _localTransport.Publish(p.Msg);
                Sender.Tell(PublishAck.Instance);
            });
            Receive<Subscribe>(s =>
            {
                _localTransport.Subscribe(s.Topic, s.Actor, s.Notificator);
                Sender.Tell(SubscribeAck.Instance);
            });
            Receive<Unsubscribe>(us =>
            {
                _localTransport.Unsubscribe(us.Actor, us.Topic);
                Sender.Tell(UnsubscribeAck.Instance);
            });
        }
    }
}