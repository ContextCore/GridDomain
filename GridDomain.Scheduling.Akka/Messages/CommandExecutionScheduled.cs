using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class CommandExecutionScheduled
    {
        public CommandExecutionScheduled(string commandId, DateTime nextExecution)
        {
            CommandId = commandId;
            NextExecution = nextExecution;
        }

        public string CommandId { get; }
        public DateTime NextExecution { get; }
    }
}