using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events
{
    public class GotTiredEvent: DomainEvent
    {
        public GotTiredEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}