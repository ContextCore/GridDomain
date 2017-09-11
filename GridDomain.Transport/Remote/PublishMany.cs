namespace GridDomain.Transport.Remote
{
    public class PublishMany
    {
        public Publish[] Messages;

        public PublishMany(params Publish[] messages)
        {
            Messages = messages;
        }
    }
}