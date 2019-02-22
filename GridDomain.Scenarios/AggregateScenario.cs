using System.Collections.Generic;
using System.Linq;
using GridDomain.Aggregates;
using GridDomain.Scenarios.Builders;

namespace GridDomain.Scenarios
{
    public class AggregateScenario<T> : AggregateScenario, IAggregateScenario<T> where T : IAggregate
    {
        public AggregateScenario(IReadOnlyCollection<IDomainEvent> givenEvents,
                                 IReadOnlyCollection<ICommand> givenCommands,
                                 IReadOnlyCollection<IDomainEvent> expectedEvents,
                                 IAggregateDependencies<T> factory,
                                 string name) : base(givenEvents, givenCommands, expectedEvents, name)
        {
            Dependencies = factory;
        }
        public IAggregateDependencies<T> Dependencies { get; }
    }



    public class AggregateScenario : IAggregateScenario
    {

        public static IAggregateScenarioBuilder<T> New<T>() where T : IAggregate
        {
            return new AggregateScenarioBuilder<T>();
        }
        public static IAggregateScenarioBuilder<T> New<T>(IAggregateDependencies<T> dependencies) where T : IAggregate
        {
            return new AggregateScenarioBuilder<T>().With(dependencies);
        }
       

        public AggregateScenario(IReadOnlyCollection<IDomainEvent> givenEvents,
                                 IReadOnlyCollection<ICommand> givenCommands,
                                 IReadOnlyCollection<IDomainEvent> expectedEvents,
                                 string name)
        {
            GivenEvents = givenEvents;
            GivenCommands = givenCommands;
            ExpectedEvents = expectedEvents;
            Name = name;

            string commandAggregateId = null;
            string eventsAggregateId = null;

            if (GivenCommands != null && GivenCommands.Any())
            {
                var command = GivenCommands.First();

                var aggregateName = command.AggregateName;
                commandAggregateId = command.AggregateId;

                if (GivenCommands.Any(c => c.AggregateId != commandAggregateId))
                    throw new CommandsBelongToDifferentAggregateIdsException();

                if (GivenCommands.Any(c => c.AggregateName != aggregateName))
                    throw new CommandsBelongToDifferentAggregateTypesException(GivenCommands);
            }

            if (GivenEvents != null && GivenEvents.Any())
            {
                eventsAggregateId = GivenEvents.First()
                                               .SourceId;
                if (GivenEvents.Any(c => c.SourceId != eventsAggregateId))
                    throw new GivenEventsBelongToDifferentAggregateTypesException();
            }


            if (commandAggregateId != null && eventsAggregateId != null && commandAggregateId != eventsAggregateId)
                throw new CommandsAndEventsHasDifferentAggregateIdsException();

            AggregateId = eventsAggregateId ?? commandAggregateId;

//            if (AggregateId == null)
//                throw new CannotDetermineAggregateIdException();

//            if (ExpectedEvents != null && ExpectedEvents.Any(e => e.SourceId != AggregateId))
 //               throw new ExpectedEventsBelongToDifferentAggregateTypesException();
        }

        public IReadOnlyCollection<IDomainEvent> ExpectedEvents { get; }
        public IReadOnlyCollection<IDomainEvent> GivenEvents { get; }
        public IReadOnlyCollection<ICommand> GivenCommands { get; }
        public string AggregateId { get; }

        public string Name { get; }
    }
}