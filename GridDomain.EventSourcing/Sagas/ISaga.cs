using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISaga<TState> where TState : ISagaState
    {
        TState State { get; }
        Task<TransitionResult<TState>> PreviewTransit<T>(T message) where T : class;
        void ApplyTransit(TState newState);
    }
}