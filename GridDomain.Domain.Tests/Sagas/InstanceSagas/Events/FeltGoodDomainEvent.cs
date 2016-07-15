using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.InstanceSagas.Events
{
    public class FeltGoodDomainEvent :DomainEvent
    {
        public Guid SubscriptionId  => SourceId;

        public FeltGoodDomainEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}
