using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleCommand
    {
        public Command Command { get; }
        public IMessageMetadata CommandMetadata { get; }
        public ScheduleKey Key { get; }
        public ExtendedExecutionOptions Options { get; }

        public ScheduleCommand(Command command, ScheduleKey key, ExtendedExecutionOptions options, IMessageMetadata commandMetadata = null)
        {
            Command = command;
            Key = key;
            Options = options;
            CommandMetadata = commandMetadata;
        }
    }
}