using Akka.Actor;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.FutureEvents;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration {
    class FutureAggregateDomainConfiguration : IDomainConfiguration
    {
        private readonly IActorRef _schedulingActor;

        public FutureAggregateDomainConfiguration(IActorRef schedulingActor)
        {
            _schedulingActor = schedulingActor;
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new FutureAggregateDependenciesFactory());

            var handler = new FutureEventsShedulingMessageHandler(_schedulingActor);
            builder.RegisterHandler<FutureEventScheduledEvent, FutureEventsShedulingMessageHandler>(c => handler).AsSync();
            builder.RegisterHandler<FutureEventCanceledEvent, FutureEventsShedulingMessageHandler>(c => handler).AsSync();
        }
    }
}