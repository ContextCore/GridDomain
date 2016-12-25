namespace GridDomain.CQRS
{
    public interface IMessageWaiter<T> : IMessageWaiterBase<T,IExpectBuilder<T>>
    {
    }
}