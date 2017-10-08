using System;

namespace GridDomain.EventSourcing {
    public static class CommandAggregateHandler
    {
        public static IAggregateCommandsHandler<T> New<T>() where T : CommandAggregate
        {
            return new ConventionAggregateHandler<T>(AggregateFactory.BuildEmpty<T>((Guid?) Guid.Empty));
        }
    }
}