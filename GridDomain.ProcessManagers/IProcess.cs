using System.Threading.Tasks;

namespace GridDomain.ProcessManagers {
    public interface IProcess<TState> where TState: IProcessState
    {
        Task<ProcessResult<TState>> Transit(TState state, object domainMessage);
    }
}