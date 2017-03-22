using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public interface ISagaStateCommand<TSagaState> : ICommand
    {
        TSagaState State { get; }
    }
}