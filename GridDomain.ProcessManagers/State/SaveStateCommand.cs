using System;
using GridDomain.CQRS;

namespace GridDomain.ProcessManagers.State
{
    public class SaveStateCommand<TState> : Command, IProcessStateCommand<TState> where TState : IProcessState
    {
        public TState State { get; }
        public Guid MessageId { get; }
        public SaveStateCommand(Guid aggregateId, TState state, Guid messageId) : base(aggregateId)
        {
            State = state;
            MessageId = messageId;
        }
    }
}