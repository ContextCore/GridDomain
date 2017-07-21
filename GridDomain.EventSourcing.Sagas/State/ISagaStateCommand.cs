using GridDomain.CQRS;

namespace GridDomain.Processes.State
{
    public interface ISagaStateCommand<TSagaState> : ICommand
    {
        TSagaState State { get; }
    }
}