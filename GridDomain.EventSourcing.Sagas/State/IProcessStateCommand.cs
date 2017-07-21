using GridDomain.CQRS;

namespace GridDomain.Processes.State
{
    public interface IProcessStateCommand<TState> : ICommand
    {
        TState State { get; }
    }
}