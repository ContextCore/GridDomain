using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class PersonCreated : DomainEvent
    {
        public PersonCreated(string sourceId, string personId) : base(sourceId)
        {
            PersonId = personId;
        }

        public string PersonId { get; private set; }
    }
}