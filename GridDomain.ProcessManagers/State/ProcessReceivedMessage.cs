using System;

namespace GridDomain.ProcessManagers.State
{
    public class ProcessReceivedMessage<TState> : ProcessStateEvent
    {
        public ProcessReceivedMessage(string sourceId, TState state, string messageId)
            : base(sourceId)
        {
            State = state;
            MessageId = messageId;
        }

        public string MessageId { get; }
        public TState State { get; }
    }
}