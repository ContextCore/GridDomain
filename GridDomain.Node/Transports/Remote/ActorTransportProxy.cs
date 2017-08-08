using System.Linq;
using Akka.Actor;

namespace GridDomain.Node.Transports.Remote
{
    public class ActorTransportProxy : ReceiveActor
    {
        public ActorTransportProxy(IActorTransport localTransport)
        {
            Receive<Publish>(p =>
                             {
                                 localTransport.Publish(p.Msg);
                                 Sender.Tell(PublishAck.Instance);
                             });
            Receive<PublishMany>(p =>
                                 {
                                     var messages = p.Messages.Select(m => m.Msg).ToArray();
                                     localTransport.Publish(messages);
                                     Sender.Tell(PublishManyAck.Instance);
                                 });
            Receive<Subscribe>(s =>
                               {
                                   localTransport.Subscribe(s.Topic, s.Actor, s.Notificator);
                                   Sender.Tell(SubscribeAck.Instance);
                               });
            Receive<Unsubscribe>(us =>
                                 {
                                     localTransport.Unsubscribe(us.Actor, us.Topic);
                                     Sender.Tell(UnsubscribeAck.Instance);
                                 });
        }
    }
}