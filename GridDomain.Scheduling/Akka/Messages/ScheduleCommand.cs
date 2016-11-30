using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleCommand
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public ExtendedExecutionOptions Options { get; }

        public ScheduleCommand(Command command, ScheduleKey key, ExtendedExecutionOptions options)
        {
            Command = command;
            Key = key;
            Options = options;
        }
    }
}