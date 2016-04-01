namespace GridDomain.CQRS.Quering
{
    public interface IGenericQuery<TRes>
    {
        TRes Execute();
    }
}