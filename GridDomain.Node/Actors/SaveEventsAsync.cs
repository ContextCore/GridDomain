using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Node.Actors
{
    class SaveEventsAsync
    {
        public SaveEventsAsync(AggregateBase newState, Action continuation)
        {
            Continuation = continuation;
            NewState = newState;
        }

        public Action Continuation { get; }
        public AggregateBase NewState { get; }
    }
}