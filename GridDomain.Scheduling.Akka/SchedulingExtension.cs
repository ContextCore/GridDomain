using Akka.Actor;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Akka {
    public class SchedulingExtension : IExtension
    {
        public IActorRef SchedulingActor { get; internal set; }

        public IScheduler Scheduler { get; internal set; }
    }
}