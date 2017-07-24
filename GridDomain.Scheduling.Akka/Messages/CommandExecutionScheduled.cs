using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class CommandExecutionScheduled
    {
        public CommandExecutionScheduled(Guid commandId, DateTime nextExecution)
        {
            CommandId = commandId;
            NextExecution = nextExecution;
        }

        public Guid CommandId { get; }
        public DateTime NextExecution { get; }
    }
}