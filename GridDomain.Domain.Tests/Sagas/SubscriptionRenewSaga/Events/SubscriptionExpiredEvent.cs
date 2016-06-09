using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events
{
    internal class SubscriptionExpiredEvent: DomainEvent
    {
        public SubscriptionExpiredEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}