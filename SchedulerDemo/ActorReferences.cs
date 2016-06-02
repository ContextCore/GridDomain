using Akka.Actor;

namespace SchedulerDemo
{
    public static class ActorReferences
    {
        public static IActorRef Reader { get; set; }
        public static IActorRef Writer { get; set; }
        public static IActorRef Scheduler { get; set; }
        public static IActorRef Handler { get; set; }
        public static IActorRef CommandManager { get; set; }
    }
}