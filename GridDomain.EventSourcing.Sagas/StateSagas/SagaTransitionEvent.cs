using System;

namespace GridDomain.EventSourcing.Sagas.StateSagas
{
    [Obsolete("Use Saga classes instead")]
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