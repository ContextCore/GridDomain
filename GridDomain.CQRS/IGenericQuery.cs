namespace GridDomain.CQRS
{
    public interface IGenericQuery<TRes>
    {
        TRes Execute();
    }
}