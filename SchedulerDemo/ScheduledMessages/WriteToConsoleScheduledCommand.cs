using System;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class DemoEvent : DomainEvent
    {
        public DemoEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }

    public class DemoCommand : ScheduledCommand
    {
        public Guid Guid { get; }

        public DemoCommand(Guid guid, string taskId, string group) : base(taskId, group)
        {
            Guid = guid;
        }
    }

    public class WriteToConsoleScheduledCommand : ScheduledCommand
    {
        public WriteToConsoleScheduledCommand(string taskId, string group) : base(taskId, group)
        {

        }
    }
}