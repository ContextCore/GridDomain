using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Schedule
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public ExecutionOptions Options { get; }

        public Schedule(Command command, ScheduleKey key, ExecutionOptions options)
        {
            Command = command;
            Key = key;
            Options = options;
        }
    }
}