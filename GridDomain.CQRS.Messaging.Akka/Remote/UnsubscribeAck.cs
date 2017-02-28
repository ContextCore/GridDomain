namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class UnsubscribeAck
    {
        public static UnsubscribeAck Instance = new UnsubscribeAck();
        private UnsubscribeAck() {}
    }
}