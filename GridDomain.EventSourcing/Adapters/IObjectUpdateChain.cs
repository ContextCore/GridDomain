namespace GridDomain.EventSourcing.Adapters
{
    public interface IObjectUpdateChain
    {
        object[] Update(object evt);
    }
}