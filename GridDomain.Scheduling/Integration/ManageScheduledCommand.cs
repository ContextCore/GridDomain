using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class ManageScheduledCommand
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }

        public ManageScheduledCommand(Command command, ScheduleKey key)
        {
            Command = command;
            Key = key;
        }
    }
}