using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class Slept : DomainEvent
    {
        public Slept(Guid aggregateId) : base(aggregateId)
        {
        }

    }
}