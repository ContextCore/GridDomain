using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    /// <summary>
    /// Marker interface just to simplify navigation from command to its Aggregate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IForAggregate<T>
    {
        
    }
    
    public class Command<T> : Command
    {
        protected Command(string id, string aggregateId, string processId, DateTime time) : base(id, aggregateId, processId, time,typeof(T).Name) { }
        protected Command(string id, string aggregateId, string processId) : base(id, aggregateId, processId,typeof(T).Name) { }
        protected Command(string id, string aggregateId, DateTime time) : base(id, aggregateId, time,typeof(T).Name) { }
        protected Command(string id, string aggregateId) : base(id, aggregateId) { }
        protected Command(string aggregateId) : base(aggregateId, typeof(T).Name) { }
    }
    
    public class Command : ICommand
    {
        protected Command(string id, string aggregateId, string processId, DateTime time, string aggregateType)
        {
            Id = id;
            Time = time;
            ProcessId = processId;
            AggregateId = aggregateId;
            AggregateType = aggregateType;
        }

        public string AggregateType { get; private set; }

        protected Command(string id, string aggregateId, string processId, string aggregateType) : this(id, aggregateId, processId, BusinessDateTime.UtcNow,aggregateType) {}

        protected Command(string id, string aggregateId, DateTime time,string aggregateType) : this(id, aggregateId, Guid.Empty.ToString(), time,aggregateType) {}

        protected Command(string id, string aggregateId,string aggregateType) : this(id, aggregateId, BusinessDateTime.UtcNow,aggregateType) {}

        protected Command(string aggregateId, string aggregateType) : this(Guid.NewGuid().ToString(), aggregateId, aggregateType) {}

        public DateTime Time { get; private set; }
        public string Id { get; private set; }
        public string ProcessId { get;  set; }
        public string AggregateId { get; }

        //TODO: think how to avoid cloning just for process id set
        public Command CloneForProcess(string processId)
        {
            var copy = (Command) MemberwiseClone();
            copy.ProcessId = processId;
            return copy;
        }
    }
}