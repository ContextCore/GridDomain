using System;

namespace GridDomain.Processes.State
{
    public class ProcessManagerCreated<TState> : ProcessStateEvent
    {
        public ProcessManagerCreated(TState state, Guid sourceId) : base(sourceId)
        {
            State = state;
        }

        public TState State { get; }
    }
}