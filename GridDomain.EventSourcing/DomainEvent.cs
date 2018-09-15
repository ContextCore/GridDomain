using System;
using GridDomain.Common;

namespace GridDomain.EventSourcing
{
    public class DomainEvent<T> : DomainEvent, IFor<T>
    {
        protected DomainEvent(string sourceId, string processId = null, string id = null, DateTime? createdTime = null, int sequence = 0) : base(sourceId, processId, id, createdTime) { }
    }

    public class DomainEvent : ISourcedEvent, IHaveId, IHaveProcessId
    {
        protected DomainEvent(string sourceId, string processId = null, string id = null, DateTime? createdTime = null, int sequence=0)
        {
            SourceId = sourceId;
            CreatedTime = createdTime ?? BusinessDateTime.UtcNow;
            ProcessId = processId;
            Sequence = sequence;
            Id = id ?? Guid.NewGuid().ToString();
        }

        //Source of the event - aggregate that created it
        //private setter for serializers
        public string SourceId { get; private set; }
        public string ProcessId { get; internal set; }
        public int Sequence { get; }
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

        public DomainEvent CloneForProcess(string processId)
        {
            var evt = (DomainEvent) MemberwiseClone();
            evt.ProcessId = processId;
            return evt;
        }

    }
}