using System;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    public static class SagaEventCorrelator
    {
        public static void MarkEventsBelongingToSaga(IAggregate aggregate, Guid sagaId)
        {
            foreach (var e in aggregate.GetUncommittedEvents().Cast<DomainEvent>())
                e.SagaId = sagaId;
        }
    }
}