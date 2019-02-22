using System;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public class DomainEvent<T> : DomainEvent, IFor<T>
    {
        protected DomainEvent(string sourceId, long version = 0, string id = null, DateTime? createdTime = null) : base(sourceId, version, id, createdTime) { }
    }

    public interface IDomainEvent : ISourcedEvent, IHaveId
    {
        long Version { get; }
    }
    
    public class DomainEvent : IDomainEvent
    {
        protected DomainEvent(string sourceId, long version = 0, string id = null, DateTime? createdTime = null)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? BusinessDateTime.UtcNow;
            Id = id ?? Guid.NewGuid().ToString();
        }

        //Source of the event - aggregate that created it
        //private setter for serializers
        public string SourceId { get; private set; }
        /// <summary>
        /// Version of the Aggregate to witch this event should be applied
        /// </summary>
        public long Version { get; set; }
        public DateTime CreatedTime { get; private set; }
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

//        public DomainEvent CloneForProcess(string processId)
//        {
//            var evt = (DomainEvent) MemberwiseClone();
//            evt.ProcessId = processId;
//            return evt;
//        }

    }
}