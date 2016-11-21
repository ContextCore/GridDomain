namespace GridDomain.CQRS.Messaging
{
    public interface IPublisher
    {
        void Publish(params object[] msg);
    }
}