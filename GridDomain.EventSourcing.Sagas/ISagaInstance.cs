using System;
using System.Collections.Generic;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaInstance
    {
        IReadOnlyCollection<ICommand> CommandsToDispatch { get; }
        void ClearCommandsToDispatch();
        IAggregate Data { get; }
        void Transit<T>(T message) where T : class;
    }

    public interface ISagaInstance<TSaga, TData> : ISagaInstance where TData : ISagaState
    {
        new SagaDataAggregate<TData> Data { get; }
    }
}