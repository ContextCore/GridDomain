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
        Task Transit<T>(T message) where T : class;
    }

    public interface ISaga<TSaga, TData> : ISaga where TData : ISagaState
    {
        TData State { get; }
    }
}