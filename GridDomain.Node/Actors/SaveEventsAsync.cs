using System;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    class SaveEventsAsync
    {
        public SaveEventsAsync(Aggregate newState, Action continuation)
        {
            Continuation = continuation;
            NewState = newState;
        }

        public Action Continuation { get; }
        public Aggregate NewState { get; }
    }
}