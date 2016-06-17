using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Scheduled
    {
        public DateTime NextExecution { get; private set; }

        public Scheduled(DateTime nextExecution)
        {
            NextExecution = nextExecution;
        }
    }
}