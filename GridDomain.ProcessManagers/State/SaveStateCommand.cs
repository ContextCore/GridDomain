using System;
using GridDomain.CQRS;

namespace GridDomain.ProcessManagers.State
{
    public class SaveStateCommand<TState> : Command<ProcessStateAggregate<TState>>, IProcessStateCommand<TState> where TState : IProcessState
    {
        public TState State { get; }
        public string MessageId { get; }
        public SaveStateCommand(string aggregateId, TState state, string messageId) : base(aggregateId)
        {
            State = state;
            MessageId = messageId;
        }
    }
}