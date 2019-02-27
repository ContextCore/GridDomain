using System;

namespace GridDomain.Aggregates
{
    public static class AggregateAddressExtensions
    {
        public static AggregateAddress AsAddressFor<T>(this string aggregateId)
        {
            return AggregateAddress.New<T>(aggregateId);
        }

        public static AggregateAddress AsAddress(this Type type, string aggregateId)
        {
            return AggregateAddress.New(type, aggregateId);
        }
    }
}