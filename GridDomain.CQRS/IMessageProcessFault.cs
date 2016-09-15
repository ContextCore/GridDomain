namespace GridDomain.CQRS
{
    public interface IFault<T> : IFault
    {
        new T Message { get; }
    }
}