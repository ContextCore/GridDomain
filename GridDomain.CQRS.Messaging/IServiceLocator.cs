namespace GridDomain.CQRS.Messaging
{
    public interface IServiceLocator
    {
        T Resolve<T>();
    }
}