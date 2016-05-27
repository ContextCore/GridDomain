using Akka.Actor;
using Akka.Event;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka
{
    public class TargetActorProvider : ITargetActorProvider
    {
        public IActorRef Get(ProcessScheduledTaskRequest request)
        {
            return new StandardOutLogger();
        }
    }
}