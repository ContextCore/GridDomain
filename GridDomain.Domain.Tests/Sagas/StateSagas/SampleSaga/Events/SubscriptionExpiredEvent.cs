using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events
{
    public class SubscriptionExpiredEvent: DomainEvent
    {
        public SubscriptionExpiredEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}