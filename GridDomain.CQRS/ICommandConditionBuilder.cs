using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandConditionBuilder
    {
        Task<IWaitResults> Execute(TimeSpan? timeout, bool failOnAnyFault);
        ICommandConditionBuilder And<TMsg>(Predicate<TMsg> filter = null);
        ICommandConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null);
    }
}