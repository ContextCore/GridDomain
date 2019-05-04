using System.Collections.Generic;
using GridDomain.Aggregates;

namespace GridDomain.Scenarios 
{
    public interface IAggregateScenario
    {
        IReadOnlyCollection<IDomainEvent> ExpectedEvents { get; }
        IReadOnlyCollection<IDomainEvent> GivenEvents { get; }
        IReadOnlyCollection<ICommand> GivenCommands { get; }
        string AggregateId { get; }
        string Name { get;}
    }


    public interface IAggregateScenario<T> : IAggregateScenario where T:IAggregate
    {
        IAggregateConfiguration<T> Configuration { get; }
    }
}