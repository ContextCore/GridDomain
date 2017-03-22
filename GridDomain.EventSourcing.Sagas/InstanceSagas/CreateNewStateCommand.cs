using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class CreateNewStateCommand<TSagaState> : Command, ISagaStateCommand<TSagaState> where TSagaState : ISagaState
    {
        public TSagaState State { get; }
        public CreateNewStateCommand(Guid aggregateId, TSagaState state) : base(aggregateId)
        {
            State = state;
        }
    }
}