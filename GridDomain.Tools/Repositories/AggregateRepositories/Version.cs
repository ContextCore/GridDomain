using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tools.Repositories.AggregateRepositories
{
    public class Version<T>
    {
        public Version(T payload, DateTime createdAt)
        {
            Payload = payload;
            CreatedAt = createdAt;
        }

        public T Payload { get; }
        public DateTime CreatedAt { get; }
    }
}