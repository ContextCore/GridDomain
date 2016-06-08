using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Integration
{
    public class ManageMessage
    {
        public ScheduledCommand Command { get; }

        public ManageMessage(ScheduledCommand command)
        {
            Command = command;
        }
    }
}