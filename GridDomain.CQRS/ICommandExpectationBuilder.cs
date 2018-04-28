using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandExpectationBuilder: IExpectationBuilder<IConditionCommandExecutor>
    {
        IConditionCommandExecutor<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class; 
    }
}