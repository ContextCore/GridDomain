using System;
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
                                 IAggregateConfiguration<T> factory,
                                 string name) : base(givenEvents, 
            
            givenCommands.Any() || expectedEvents.Any() ?
            new []{new Plan(givenCommands, expectedEvents,1)} : new Plan[]{}, name)
        {
            Configuration = factory;
        }
        
        public AggregateScenario(IReadOnlyCollection<IDomainEvent> givenEvents,
            IReadOnlyCollection<Plan> plans,
            IAggregateConfiguration<T> factory,
            string name) : base(givenEvents,plans, name)
        {
            Configuration = factory;
        }
        
        public IAggregateConfiguration<T> Configuration { get; }
    }



    public class AggregateScenario : IAggregateScenario
    {

        public static IAggregateScenarioBuilder<T> New<T>() where T : IAggregate
        {
            return new AggregateScenarioBuilder<T>();
        }
        public static IAggregateScenarioBuilder<T> New<T>(IAggregateConfiguration<T> configuration) where T : IAggregate
        {
            return new AggregateScenarioBuilder<T>().With(configuration);
        }
       

        public AggregateScenario(IReadOnlyCollection<IDomainEvent> givenEvents,
                                 IReadOnlyCollection<Plan> plans,
                                 string name)
        {
            GivenEvents = givenEvents;
            Name = name;
            Plans = plans;
            
            string commandAggregateId = null;
            string eventsAggregateId = null;

            var givenCommands = plans.FirstOrDefault()?.GivenCommands;
                
            if (givenCommands != null && givenCommands.Any())
            {
                var command = givenCommands.First();

                commandAggregateId = command.Recipient.Id;

                if (givenCommands.Any(c => c.Recipient.Id != commandAggregateId))
                    throw new CommandsBelongToDifferentAggregateIdsException();

                if (givenCommands.Any(c => c.Recipient.Name != command.Recipient.Name))
                    throw new CommandsBelongToDifferentAggregateTypesException(givenCommands);
            }

            if (GivenEvents != null && GivenEvents.Any())
            {
                eventsAggregateId = GivenEvents.First()
                                               .Source.Id;
                if (GivenEvents.Any(c => c.Source.Id != eventsAggregateId))
                    throw new GivenEventsBelongToDifferentAggregateTypesException();
            }


            if (commandAggregateId != null && eventsAggregateId != null && commandAggregateId != eventsAggregateId)
                throw new CommandsAndEventsHasDifferentAggregateIdsException();

            AggregateId = eventsAggregateId ?? commandAggregateId;

        }

        public IReadOnlyCollection<IDomainEvent> GivenEvents { get; }
        public IReadOnlyCollection<Plan> Plans { get; }
        public string AggregateId { get; }

        public string Name { get; }
    }
}