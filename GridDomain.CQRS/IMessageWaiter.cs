using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    //public interface IMessageWaiter<T> : IExpectationBuilder<IConditionFactory<T>>
    //{
    //}

    public interface IMessageWaiter<out TFactory> : IExpectationBuilder<TFactory> where TFactory : IMessageFilter<TFactory>
    {
        
    }

    public interface IMessageWaiter : IMessageWaiter<IConditionFactory<Task<IWaitResult>>>
    {
        
    }
}