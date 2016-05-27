using Akka.Actor;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka
{
    public interface ITargetActorProvider
    {
        IActorRef Get(ProcessScheduledTaskRequest request);
    }
}