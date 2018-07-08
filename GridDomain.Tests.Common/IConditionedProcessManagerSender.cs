using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Tests.Common {
    public interface IConditionedProcessManagerSender<T> : IConditionedProcessManagerSender
    {
        new Task<IWaitResult<T>> Send(TimeSpan? timeout = null, bool failOnAnyFault = true);
    }
}