using System;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    public class SagaTransitionEvents<TTransition> : DomainEvent
    {
        public SagaTransitionEvents(TTransition transition, Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
            Transition = transition;
        }

        public TTransition Transition { get; }
    }
}