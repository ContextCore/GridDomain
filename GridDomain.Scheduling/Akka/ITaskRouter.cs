using Akka.Actor;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka
{
    public interface ITaskRouter
    {
        IActorRef GetTarget(ProcessScheduledTaskRequest request);
    }
}