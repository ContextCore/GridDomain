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
        protected Command(Guid id, Guid aggregateId, Guid processId, DateTime time) : base(id, aggregateId, processId, time) { }
        protected Command(Guid id, Guid aggregateId, Guid processId) : base(id, aggregateId, processId) { }
        protected Command(Guid id, Guid aggregateId, DateTime time) : base(id, aggregateId, time) { }
        protected Command(Guid id, Guid aggregateId) : base(id, aggregateId) { }
        protected Command(Guid aggregateId) : base(aggregateId) { }
    }
    
    public class Command : ICommand
    {
        protected Command(Guid id, Guid aggregateId, Guid processId, DateTime time)
        {
            Id = id;
            Time = time;
            ProcessId = processId;
            AggregateId = aggregateId;
        }

        protected Command(Guid id, Guid aggregateId, Guid processId) : this(id, aggregateId, processId, BusinessDateTime.UtcNow) {}

        protected Command(Guid id, Guid aggregateId, DateTime time) : this(id, aggregateId, Guid.Empty, time) {}

        protected Command(Guid id, Guid aggregateId) : this(id, aggregateId, BusinessDateTime.UtcNow) {}

        protected Command(Guid aggregateId) : this(Guid.NewGuid(), aggregateId) {}

        public DateTime Time { get; private set; }
        public Guid Id { get; private set; }
        public Guid ProcessId { get; set; }
        public Guid AggregateId { get; }

        //TODO: think how to avoid cloning just for process id set
        public Command CloneForProcess(Guid processId)
        {
            var copy = (Command) MemberwiseClone();
            copy.ProcessId = processId;
            return copy;
        }
    }
}