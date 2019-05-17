using System;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public class DomainEvent<T> : DomainEvent, IFor<T>
    {
        protected DomainEvent(string source, long version = 0, string id = null, DateTime? createdTime = null) : base(AggregateAddress.New<T>(source), version, id, createdTime) { }
    }

    public class DomainEvent : IDomainEvent
    {
        protected DomainEvent(AggregateAddress source, long version = 0, string id = null, DateTime? createdTime = null)
        {
            Source = source;
            Occured = createdTime ?? DateTimeOffset.Now;
            Id = id ?? Guid.NewGuid().ToString();
            Version = version;
        }

        //Source of the event - aggregate that created it
        //private setter for serializers
        public IAggregateAddress Source { get; private set; }
        /// <summary>
        /// Version of the Aggregate by the moment of event occurrence
        /// </summary>
        public long Version { get; set; }
        public DateTimeOffset Occured { get; private set; }
        public string Id { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj!= null && obj is DomainEvent e)
            {
                return e.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}