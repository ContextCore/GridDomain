namespace GridDomain.Common
{
    public interface ICatalog<out TData, in TMessage>
    {
        TData Get(TMessage evt);
    }
}