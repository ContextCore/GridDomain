using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandWaiter
    {
        ICommandExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null);
        ICommandExpectBuilder Expect(Type type, Func<object, bool> filter = null);
        Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}