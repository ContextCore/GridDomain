using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.ProcessManagers {
    public interface IProcess<in TState> where TState: IProcessState
    {
        Task<IReadOnlyCollection<ICommand>> Transit(TState state, object domainMessage);
    }
}