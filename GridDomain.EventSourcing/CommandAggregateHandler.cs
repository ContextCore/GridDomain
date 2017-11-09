using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public static class CommandAggregateHandler
    {
        private static IConstructAggregates DefaultFactory = new AggregateFactory();
        public static IAggregateCommandsHandler<T> New<T>(IConstructAggregates factory = null) where T : CommandAggregate
        {
            return new ConventionAggregateHandler<T>((factory ?? DefaultFactory).BuildEmpty<T>());
        }
    }
}