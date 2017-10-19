using System;

namespace GridDomain.ProcessManagers.State
{
    public class ProcessReceivedMessage<TState> : ProcessStateEvent
    {
        public ProcessReceivedMessage(Guid sourceId, TState state, Guid messageId)
            : base(sourceId)
        {
            State = state;
            MessageId = messageId;
        }

        public Guid MessageId { get; }
        public TState State { get; }
    }
}