using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public static class CommandAggregateHandler
    {
        public static IAggregateCommandsHandler<T> New<T>(IConstructAggregates factory = null) where T : CommandAggregate
        {
            return new ConventionAggregateHandler<T>((T)(factory ?? AggregateFactory.Default).Build(typeof(T),null,null));
        }
    }
}