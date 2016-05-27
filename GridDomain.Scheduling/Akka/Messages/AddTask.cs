using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class AddTask
    {
        public AkkaScheduledTask Task { get; }

        public AddTask(AkkaScheduledTask task)
        {
            Task = task;
        }
    }
}