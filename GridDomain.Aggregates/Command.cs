using System;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public class Command : ICommand
    {
        protected Command(string id, string aggregateId, string processId, DateTime time, string aggregateName)
        {
            Id = id;
            Time = time;
            ProcessId = processId;
            AggregateId = aggregateId;
            AggregateName = aggregateName;
        }

        public string AggregateName { get; private set; }

        protected Command(string id, string aggregateId, string processId, string aggregateType) : this(id, aggregateId, processId, BusinessDateTime.UtcNow, aggregateType) {}

        protected Command(string id, string aggregateId, DateTime time,string aggregateType) : this(id, aggregateId, null, time, aggregateType) {}

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