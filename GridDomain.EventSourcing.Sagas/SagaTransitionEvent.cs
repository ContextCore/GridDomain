using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaTransitionEvent<TState, TTransition> : SagaStateEvent
    {
        public SagaTransitionEvent(TTransition transition, TState state, Guid sourceId)
            : base(sourceId)
        {
            Transition = transition;
            State = state;
        }

        public TTransition Transition { get; }
        public TState State { get; }
    }
}