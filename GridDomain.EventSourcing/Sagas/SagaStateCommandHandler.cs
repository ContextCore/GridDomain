
namespace GridDomain.EventSourcing.Sagas
{
    public class SagaStateCommandHandler<TSagaState> : AggregateCommandsHandler<SagaStateAggregate<TSagaState>> where TSagaState : ISagaState
    {
        public SagaStateCommandHandler()
        {
            Map<SaveStateCommand<TSagaState>>((c, a) => a.ReceiveMessage(c.State, c.Message));
            Map<CreateNewStateCommand<TSagaState>>(c => new SagaStateAggregate<TSagaState>(c.State));
        }
    }
}