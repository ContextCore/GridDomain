using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events
{
    public class SubscriptionPaidEvent :DomainEvent
    {
        public Guid SubscriptionId  => SourceId;

        public SubscriptionPaidEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}
