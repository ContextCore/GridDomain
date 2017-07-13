using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Scheduling.FutureEvents;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration {
    class FutureAggregateHandlersDomainConfiguration : IDomainConfiguration
    {
        private readonly IActorRef _schedulingActor;

        public FutureAggregateHandlersDomainConfiguration(IActorRef schedulingActor)
        {
            _schedulingActor = schedulingActor;
        }
        public void Register(IDomainBuilder builder)
        {
            var handler = new FutureEventsShedulingMessageHandler(_schedulingActor);
            builder.RegisterHandler<FutureEventScheduledEvent, FutureEventsShedulingMessageHandler>(c => handler).AsSync();
            builder.RegisterHandler<FutureEventCanceledEvent, FutureEventsShedulingMessageHandler>(c => handler).AsSync();
        }
    }
}