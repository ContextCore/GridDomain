using System;
using CommonDomain;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class AggregateVersion<T> where T : IAggregate
    {
        public AggregateVersion(T aggregate, DateTime createdAt)
        {
            Aggregate = aggregate;
            CreatedAt = createdAt;
        }

        public T Aggregate { get; }
        public DateTime CreatedAt { get; }
    }
}