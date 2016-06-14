using System;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Tasks
{
    public abstract class ScheduledCommand : Command
    {
        public string Group { get;  }
        public string TaskId { get; }

        //TODO::VZ:: remove
        public IActorRef Manager { get; set; }
        protected ScheduledCommand(string taskId, string group)
        {
            if (string.IsNullOrWhiteSpace(taskId))
            {
                throw new ArgumentException(nameof(taskId));
            }

            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException(nameof(group));
            }
            Group = group;
            TaskId = taskId;
        }
    }
}
