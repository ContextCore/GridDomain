using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISaga
    {
        IReadOnlyCollection<ICommand> CommandsToDispatch { get; }
        ISagaState State { get; } 
        void ClearCommandsToDispatch();
        Task<ISagaState> CreateNextState<T>(T message) where T : class;
    }

    public interface ISaga<TSaga, TState> : ISaga where TState : ISagaState
    {
        TState State { get; }
        new Task<TState> CreateNextState<T>(T message) where T : class;

    }
}