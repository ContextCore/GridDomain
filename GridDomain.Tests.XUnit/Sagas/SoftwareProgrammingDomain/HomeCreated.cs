using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class HomeCreated : DomainEvent
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }

        public HomeCreated(Guid id, Guid personId):base(id)
        {
            Id = id;
            PersonId = personId;
        }
    }
}