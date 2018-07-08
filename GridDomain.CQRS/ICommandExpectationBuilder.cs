using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandExpectationBuilder: IExpectationBuilder<ICommandEventsFilter>
    {
        ICommandEventsFilter<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class; 
    }
}