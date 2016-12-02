namespace GridDomain.CQRS
{
    public interface IFault<out T> : IFault
    {
        new T Message { get; }
    }
}