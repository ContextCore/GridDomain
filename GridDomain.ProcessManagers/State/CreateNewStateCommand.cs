using System;
using GridDomain.CQRS;

namespace GridDomain.ProcessManagers.State
{
    public class CreateNewStateCommand<TState> : Command<ProcessStateAggregate<TState>>, IProcessStateCommand<TState> where TState : IProcessState
    {
        public TState State { get; }
        public CreateNewStateCommand(string aggregateId, TState state) : base(aggregateId)
        {
            State = state;
        }
    }
}