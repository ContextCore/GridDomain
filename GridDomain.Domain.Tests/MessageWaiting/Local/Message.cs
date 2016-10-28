namespace GridDomain.Tests.MessageWaiting.Local
{
    public class Message
    {
        public Message(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}