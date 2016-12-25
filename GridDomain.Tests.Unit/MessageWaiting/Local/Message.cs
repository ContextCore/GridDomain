namespace GridDomain.Tests.Unit.MessageWaiting.Local
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