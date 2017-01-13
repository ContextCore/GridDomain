using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandExpectBuilder
    {
        Task<IWaitResults> Execute(TimeSpan? timeout, bool failOnAnyFault);
        ICommandExpectBuilder And<TMsg>(Predicate<TMsg> filter = null);
        ICommandExpectBuilder Or<TMsg>(Predicate<TMsg> filter = null);
    }
}