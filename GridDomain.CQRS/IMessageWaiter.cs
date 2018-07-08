using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IMessageWaiter<out TFactory> : IExpectationBuilder<TFactory> where TFactory : IMessageFilter<TFactory>
    {
        
    }

    public interface IMessageWaiter : IMessageWaiter<IMessageConditionFactory<Task<IWaitResult>>>
    {
        
    }
}