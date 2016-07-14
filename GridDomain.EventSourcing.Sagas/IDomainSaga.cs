using System.Collections.Generic;
using CommonDomain;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaInstance
    {
        IReadOnlyCollection<object> CommandsToDispatch { get; }
        void ClearCommandsToDispatch();
        IAggregate Data { get; }
        void Transit(object message);
        void Transit<T>(T message) where T : class;
    }
}