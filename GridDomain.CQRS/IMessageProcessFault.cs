namespace GridDomain.CQRS
{
    public interface IMessageFault<T> : IMessageFault
    {
        new T Message { get; }
    }
}