using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events
{
    public class GotMoreTiredEvent :DomainEvent
    {
        public GotMoreTiredEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}