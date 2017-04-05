using System;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SaveStateCommand<TSagaState> : Command, ISagaStateCommand<TSagaState> where TSagaState : ISagaState
    {
        public TSagaState State { get; }
        public string PreviousState { get; }
        public object Message { get; }
        public SaveStateCommand(Guid aggregateId, TSagaState state, string previousState, object message) : base(aggregateId)
        {
            State = state;
            PreviousState = previousState;
            Message = message;
        }
    }
}