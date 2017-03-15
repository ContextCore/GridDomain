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

    }

    public interface ISaga<TSaga, TState> : ISaga where TState : ISagaState
    {
        TState State { get; set; }
        Task<StatePreview<TState>> CreateNextState<T>(T message) where T : class;
    }
}