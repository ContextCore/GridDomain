using System.Collections.Generic;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios 
{
    public interface IAggregateScenario
    {
        IReadOnlyCollection<IDomainEvent> GivenEvents { get; }
        IReadOnlyCollection<Plan> Plans { get; }
        string AggregateId { get; }
        string Name { get;}
    }

    public class Plan
    {
        public Plan(IReadOnlyCollection<ICommand> givenCommands, IReadOnlyCollection<IDomainEvent> expectedEvents, int step)
        {
            GivenCommands = givenCommands;
            ExpectedEvents = expectedEvents;
            Step = step;
        }

        public IReadOnlyCollection<ICommand> GivenCommands { get; }
        public IReadOnlyCollection<IDomainEvent> ExpectedEvents { get; }
        public int Step { get;}
    }


    public interface IAggregateScenario<T> : IAggregateScenario where T:IAggregate
    {
        IAggregateConfiguration<T> Configuration { get; }
    }
}