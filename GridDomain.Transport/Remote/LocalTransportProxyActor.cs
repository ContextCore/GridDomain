using System.Linq;
using Akka.Actor;

namespace GridDomain.Transport.Remote
{
    public class LocalTransportProxyActor : ReceiveActor
    {
        public LocalTransportProxyActor()
        {
            var localTransport = new LocalAkkaEventBusTransport(Context.System);
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