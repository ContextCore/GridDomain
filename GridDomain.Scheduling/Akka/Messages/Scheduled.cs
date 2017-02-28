using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Scheduled
    {
        public Scheduled(DateTime nextExecution)
        {
            NextExecution = nextExecution;
        }

        public DateTime NextExecution { get; private set; }
    }
}