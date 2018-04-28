using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IMessageWaiter<T> : IExpectationBuilder<IConditionFactory<T>>
    {
    }
}