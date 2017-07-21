using GridDomain.EventSourcing;

namespace GridDomain.Processes.State
{
    public class ProcessStateCommandHandler<TSagaState> : AggregateCommandsHandler<ProcessStateAggregate<TSagaState>> where TSagaState : IProcessState
    {
        public ProcessStateCommandHandler()
        {
            Map<SaveStateCommand<TSagaState>>((c, a) => a.ReceiveMessage(c.State, c.Message));
            Map<CreateNewStateCommand<TSagaState>>(c => new ProcessStateAggregate<TSagaState>(c.State));
        }
    }
}