using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public static class ConstructAggregatesExtensions
    {
        public static T BuildEmpty<T>(this IConstructAggregates factory,string id = null) where T : IAggregate
        {
            return (T)factory.Build(typeof(T),id ?? Guid.NewGuid().ToString(),null);
        }
    }
}