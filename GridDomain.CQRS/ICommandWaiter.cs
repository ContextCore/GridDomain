using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandWaiter
    {
        ICommandConditionBuilder Expect<TMsg>(Predicate<TMsg> filter = null);
        ICommandConditionBuilder Expect(Type type, Func<object, bool> filter = null);
        Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}