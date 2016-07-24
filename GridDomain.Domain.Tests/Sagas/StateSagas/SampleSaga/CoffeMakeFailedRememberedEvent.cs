using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class CoffeMakeFailedRememberedEvent : DomainEvent
    {
        public CoffeMakeFailedEvent E { get; private set; }

        public CoffeMakeFailedRememberedEvent(Guid sagaId, CoffeMakeFailedEvent e) : base(sagaId)
        {
            E = e;
        }
    }
}