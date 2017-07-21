using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Processes.State
{
    public class ProcessStateAggregate<TState> : Aggregate where TState : IProcessState
    {
        public ProcessStateAggregate(TState state): this(state.Id)
        {
            Condition.NotNull(() => state);
            Produce(new ProcessManagerCreated<TState>(state, state.Id));
        }
        
        private ProcessStateAggregate(Guid id) : base(id)
        {
        }

        public TState State { get; private set; }

        public void ReceiveMessage(TState state, object message)
        {
            Produce(new ProcessReceivedMessage<TState>(Id, state, message));
        }

        public void Apply(ProcessManagerCreated<TState> e)
        {
            State = e.State;
            Id = e.SourceId;
        }

        public void Apply(ProcessReceivedMessage<TState> e)
        {
            State = e.State;
        }
    }
}