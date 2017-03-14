using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class HomeCreated : DomainEvent
    {
        public HomeCreated(Guid sourceId, Guid personId) : base(sourceId)
        {
            PersonId = personId;
        }

        public Guid PersonId { get; private set; }
    }
}