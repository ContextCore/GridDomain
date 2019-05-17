using System;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Aggregates
{
    public static class AggregateFactoryExtensions
    {
        public static T Build<T>(this IAggregateFactory factory, string id=null) where T : IAggregate
        {
            return (T)factory.Build(typeof(T), id??Guid.NewGuid().ToString());
        }
    }
}