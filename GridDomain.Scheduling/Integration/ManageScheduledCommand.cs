using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Integration
{
    public class ManageScheduledCommand
    {
        public ScheduledCommand Command { get; }

        public ManageScheduledCommand(ScheduledCommand command)
        {
            Command = command;
        }
    }
}