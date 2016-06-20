using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleCommand
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public ExecutionOptions Options { get; }

        public ScheduleCommand(Command command, ScheduleKey key, ExecutionOptions options)
        {
            Command = command;
            Key = key;
            Options = options;
        }
    }
}