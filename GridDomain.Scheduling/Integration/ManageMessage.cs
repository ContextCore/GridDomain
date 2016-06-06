using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Integration
{
    public class ManageMessage
    {
        public ScheduledMessage Message { get; }

        public ManageMessage(ScheduledMessage message)
        {
            Message = message;
        }
    }
}