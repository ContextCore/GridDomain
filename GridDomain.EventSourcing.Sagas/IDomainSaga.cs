using System.Collections.Generic;
using CommonDomain;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IDomainSaga
    {
        IReadOnlyCollection<object> CommandsToDispatch { get; }
        void ClearCommandsToDispatch();
        IAggregate State { get; }
        void Transit(object message);
    }
}