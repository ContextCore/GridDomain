using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleCommandExecution
    {
        public ScheduleCommandExecution(Command command,
                               ScheduleKey key,
                               ExecutionOptions options,
                               IMessageMetadata commandMetadata = null)
        {
            Command = command;
            Key = key;
            Options = options;
            CommandMetadata = commandMetadata;
        }

        public Command Command { get; }
        public IMessageMetadata CommandMetadata { get; }
        public ScheduleKey Key { get; }
        public ExecutionOptions Options { get; }
    }
}