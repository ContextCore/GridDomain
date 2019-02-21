using System.Collections.Generic;
using GridDomain.Aggregates;

namespace GridDomain.Scenarios 
{
    public interface IAggregateScenario
    {
        IReadOnlyCollection<DomainEvent> ExpectedEvents { get; }
        IReadOnlyCollection<DomainEvent> GivenEvents { get; }
        IReadOnlyCollection<ICommand> GivenCommands { get; }
        string AggregateId { get; }
        string Name { get;}
    }


    public interface IAggregateScenario<T> : IAggregateScenario where T:IAggregate
    {
        IAggregateDependencies<T> Dependencies { get; }
    }
}