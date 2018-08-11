using System.Collections.Generic;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Scenarios 
{
    public interface IAggregateScenario
    {
        IReadOnlyCollection<DomainEvent> ExpectedEvents { get; }
        IReadOnlyCollection<DomainEvent> GivenEvents { get; }
        IReadOnlyCollection<ICommand> GivenCommands { get; }
        string AggregateId { get; }
    }
}