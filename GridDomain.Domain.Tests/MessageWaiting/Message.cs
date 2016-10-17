namespace GridDomain.Tests.MessageWaiting
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