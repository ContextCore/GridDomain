using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Scheduling.FutureEvents;

namespace GridDomain.Scheduling.Akka {
    public class FutureAggregateHandlersDomainConfiguration : IDomainConfiguration
    {
        private readonly IActorRef _schedulingActor;

        public FutureAggregateHandlersDomainConfiguration(IActorRef schedulingActor)
        {
            _schedulingActor = schedulingActor;
        }
        public void Register(IDomainBuilder builder)
        {
            var handler = new FutureEventsSchedulingMessageHandler(_schedulingActor);
            builder.RegisterHandler<FutureEventScheduledEvent, FutureEventsSchedulingMessageHandler>(c => handler).AsSync();
            builder.RegisterHandler<FutureEventCanceledEvent, FutureEventsSchedulingMessageHandler>(c => handler).AsSync();
        }
    }
}