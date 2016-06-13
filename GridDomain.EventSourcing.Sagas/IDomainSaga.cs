using System.Collections.Generic;
using CommonDomain;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{

    public interface IDomainStateSaga<T>: IDomainSaga where T: IAggregate
    {
        new T State { get; }
    }

    public interface IDomainSaga
    {
        IReadOnlyCollection<object> CommandsToDispatch { get; }
        void ClearCommandsToDispatch();
        IAggregate State { get; }
        void Transit(object message);
    }
}