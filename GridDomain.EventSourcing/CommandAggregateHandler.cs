using System;

namespace GridDomain.EventSourcing {
    public static class CommandAggregateHandler
    {
        public static IAggregateCommandsHandler<T> New<T>() where T : CommandAggregate
        {
            return new CommandAggregateHandler<T>(Aggregate.Empty<T>(Guid.Empty));
        }
    }
}