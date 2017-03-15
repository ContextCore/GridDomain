using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaInstance
    {
        IReadOnlyCollection<ICommand> CommandsToDispatch { get; }
        IAggregate Data { get; }
        void ClearCommandsToDispatch();
        Task Transit<T>(T message) where T : class;
    }

    public interface ISaga<TSaga, TData> : ISagaInstance where TData : ISagaState
    {
        SagaStateAggregate<TData> Data { get; }
    }
}