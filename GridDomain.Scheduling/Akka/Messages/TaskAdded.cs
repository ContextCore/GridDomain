using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class TaskAdded
    {
        public DateTime NextExecution { get; private set; }

        public TaskAdded(DateTime nextExecution)
        {
            NextExecution = nextExecution;
        }
    }
}