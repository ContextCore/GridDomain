using System;

namespace GridDomain.Aggregates
{
    public interface IAggregateAddress
    {
        string Name { get; }
        string Id { get; }
    }
}