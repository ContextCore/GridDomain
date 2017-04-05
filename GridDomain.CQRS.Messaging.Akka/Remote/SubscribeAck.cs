namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class SubscribeAck
    {
        public static readonly SubscribeAck Instance = new SubscribeAck();
        private SubscribeAck() {}
    }
}