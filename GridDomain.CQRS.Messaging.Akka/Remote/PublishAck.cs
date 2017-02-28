namespace GridDomain.CQRS.Messaging.Akka.Remote
{
    public class PublishAck
    {
        public static PublishAck Instance = new PublishAck();
        private PublishAck() {}
    }
}