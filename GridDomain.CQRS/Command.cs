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
    
    public class Command<T> : Command, IForAggregate<T>
    {
        protected Command(string id, string aggregateId, string processId, DateTime time) : base(id, aggregateId, processId, time) { }
        protected Command(string id, string aggregateId, string processId) : base(id, aggregateId, processId) { }
        protected Command(string id, string aggregateId, DateTime time) : base(id, aggregateId, time) { }
        protected Command(string id, string aggregateId) : base(id, aggregateId) { }
        protected Command(string aggregateId) : base(aggregateId) { }
    }
    
    public class Command : ICommand
    {
        protected Command(string id, string aggregateId, string processId, DateTime time)
        {
            Id = id;
            Time = time;
            ProcessId = processId;
            AggregateId = aggregateId;
            AggregateType = aggregateType;
        }

        public string AggregateType { get; set; }

        protected Command(string id, string aggregateId, string processId) : this(id, aggregateId, processId, BusinessDateTime.UtcNow) {}

        protected Command(string id, string aggregateId, DateTime time) : this(id, aggregateId, Guid.Empty.ToString(), time) {}

        protected Command(string id, string aggregateId) : this(id, aggregateId, BusinessDateTime.UtcNow) {}

        protected Command(string aggregateId) : this(Guid.NewGuid().ToString(), aggregateId) {}

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