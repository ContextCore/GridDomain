using System.Collections.Generic;
using CommonDomain;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

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

    public interface ISagaInstance<TSaga, TData> : ISagaInstance where TData : ISagaState, new()
    {
        new SagaDataAggregate<TData> Data { get; }
    }
}