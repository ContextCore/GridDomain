namespace GridDomain.CQRS.Messaging
{
    public interface IMessageConsumer<T> : IMessageFilter<T>, IHandler<T>
    {
    }
}