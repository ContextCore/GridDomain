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
            Produce(new SagaCreated<TState>(state, state.Id));
        }
        
        private ProcessStateAggregate(Guid id) : base(id)
        {
        }

        public TState State { get; private set; }

        public void ReceiveMessage(TState sagaData, object message)
        {
            Produce(new SagaReceivedMessage<TState>(Id, sagaData, message));
        }

        public void Apply(SagaCreated<TState> e)
        {
            State = e.State;
            Id = e.SourceId;
        }

        public void Apply(SagaReceivedMessage<TState> e)
        {
            State = e.State;
        }
    }
}