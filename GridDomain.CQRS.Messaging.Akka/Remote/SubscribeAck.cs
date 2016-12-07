namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class SubscribeAck
    {
        public static SubscribeAck Instance = new SubscribeAck();
        private SubscribeAck() { }
    }
}