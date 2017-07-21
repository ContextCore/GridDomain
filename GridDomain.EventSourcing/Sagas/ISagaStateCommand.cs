using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaStateCommand<TSagaState> : ICommand
    {
        TSagaState State { get; }
    }
}