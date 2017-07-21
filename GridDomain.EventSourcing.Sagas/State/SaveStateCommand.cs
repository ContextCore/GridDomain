using System;
using GridDomain.CQRS;

namespace GridDomain.Processes.State
{
    public class SaveStateCommand<TState> : Command, IProcessStateCommand<TState> where TState : IProcessState
    {
        public TState State { get; }
        public string PreviousState { get; }
        public object Message { get; }
        public SaveStateCommand(Guid aggregateId, TState state, string previousState, object message) : base(aggregateId)
        {
            State = state;
            PreviousState = previousState;
            Message = message;
        }
    }
}