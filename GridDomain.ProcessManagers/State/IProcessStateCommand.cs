using GridDomain.CQRS;

namespace GridDomain.ProcessManagers.State
{
    public interface IProcessStateCommand<TState> : ICommand
    {
        TState State { get; }
    }
}