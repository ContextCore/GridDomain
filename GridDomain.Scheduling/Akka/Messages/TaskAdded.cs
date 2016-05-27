using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public sealed class TaskAdded
    {
        public DateTime NextExecution { get; private set; }

        public TaskAdded(DateTime nextExecution)
        {
            NextExecution = nextExecution;
        }
    }
}