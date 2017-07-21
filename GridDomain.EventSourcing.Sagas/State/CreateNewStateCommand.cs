using System;
using GridDomain.CQRS;

namespace GridDomain.Processes.State
{
    public class CreateNewStateCommand<TState> : Command, IProcessStateCommand<TState> where TState : IProcessState
    {
        public TState State { get; }
        public CreateNewStateCommand(Guid aggregateId, TState state) : base(aggregateId)
        {
            State = state;
        }
    }
}