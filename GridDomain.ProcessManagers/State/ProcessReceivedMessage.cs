using System;

namespace GridDomain.ProcessManagers.State
{
    public class ProcessReceivedMessage<TState> : ProcessStateEvent
    {
        public ProcessReceivedMessage(Guid sourceId, TState state, object message)
            : base(sourceId)
        {
            State = state;
            Message = message;
        }

        //TODO: store type + message id to lower storage consumption
        public object Message { get; }
        public TState State { get; }
    }
}