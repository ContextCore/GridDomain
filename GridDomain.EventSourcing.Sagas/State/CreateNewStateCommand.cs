using System;
using GridDomain.CQRS;

namespace GridDomain.Processes.State
{
    public class CreateNewStateCommand<TSagaState> : Command, ISagaStateCommand<TSagaState> where TSagaState : IProcessState
    {
        public TSagaState State { get; }
        public CreateNewStateCommand(Guid aggregateId, TSagaState state) : base(aggregateId)
        {
            State = state;
        }
    }
}