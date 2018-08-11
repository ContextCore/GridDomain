using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Scenarios
{
    public class AggregateScenario : IAggregateScenario
    {
        public AggregateScenario(IReadOnlyCollection<DomainEvent> givenEvents,
                                 IReadOnlyCollection<ICommand> givenCommands,
                                 IReadOnlyCollection<DomainEvent> expectedEvents)
        {
            GivenEvents = givenEvents;
            GivenCommands = givenCommands;
            ExpectedEvents = expectedEvents;

            string commandAggregateId = null;
            string eventsAggregateId = null;

            if (GivenCommands != null && GivenCommands.Any())
            {
                var command = GivenCommands.First();

                var aggregateName = command.AggregateType;
                commandAggregateId = command.AggregateId;

                if (GivenCommands.Any(c => c.AggregateId != commandAggregateId))
                    throw new CommandsBelongToDifferentAggregateIdsException();

                if (GivenCommands.Any(c => c.AggregateType != aggregateName))
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

            if (AggregateId == null)
                throw new CannotDetermineAggregateIdException();

            if (ExpectedEvents != null && ExpectedEvents.Any(e => e.SourceId != AggregateId))
                throw new ExpectedEventsBelongToDifferentAggregateTypesException();
        }

        public IReadOnlyCollection<DomainEvent> ExpectedEvents { get;   }
        public IReadOnlyCollection<DomainEvent> GivenEvents { get; }
        public IReadOnlyCollection<ICommand> GivenCommands { get; }
        public string AggregateId { get; }
    }

    public class ExpectedEventsBelongToDifferentAggregateTypesException : Exception { }

    public class CommandsAndEventsHasDifferentAggregateIdsException : Exception { }

    public class GivenEventsBelongToDifferentAggregateTypesException : Exception { }

    public class CommandsBelongToDifferentAggregateIdsException : Exception { }
}