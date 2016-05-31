using Akka.Actor;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Integration
{
    public interface ITaskRouter
    {
        IActorRef GetTarget(ScheduledRequest request);
    }
}