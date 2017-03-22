namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaStateCommandHandler<TSagaState> : AggregateCommandsHandler<SagaStateAggregate<TSagaState>> where TSagaState : ISagaState
    {
        public SagaStateCommandHandler()
        {
            Map<SaveStateCommand<TSagaState>>((c, a) => a.RememberEvent(c.State, c.Message, c.MachineStatePreviousName));
            Map<CreateNewStateCommand<TSagaState>>(c => new SagaStateAggregate<TSagaState>(c.State));
        }
    }
}