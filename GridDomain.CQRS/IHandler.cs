namespace GridDomain.CQRS
{
    public interface IHandler<in T>
    {
        void Handle(T e);
    }
}