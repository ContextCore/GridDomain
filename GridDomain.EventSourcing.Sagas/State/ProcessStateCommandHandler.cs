using GridDomain.EventSourcing;

namespace GridDomain.Processes.State
{
    public class ProcessStateCommandHandler<TState> : AggregateCommandsHandler<ProcessStateAggregate<TState>> where TState : IProcessState
    {
        public ProcessStateCommandHandler()
        {
            Map<SaveStateCommand<TState>>((c, a) => a.ReceiveMessage(c.State, c.Message));
            Map<CreateNewStateCommand<TState>>(c => new ProcessStateAggregate<TState>(c.State));
        }
    }
}