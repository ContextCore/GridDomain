using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandWaiter
    {
        ICommandConditionBuilder<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class; 
        ICommandConditionBuilder Expect(Type type, Func<object, bool> filter = null);
        Task<IWaitResult> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}