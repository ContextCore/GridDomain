using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events
{
    public class FeltGoodEvent :DomainEvent
    {
        public Guid SubscriptionId  => SourceId;

        public FeltGoodEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}
