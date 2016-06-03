using System;

namespace GridDomain.Scheduling.Akka.Tasks
{
    public abstract class ScheduledMessage
    {
        public string Group { get;  }
        public string TaskId { get; }

        protected ScheduledMessage(string taskId, string group)
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