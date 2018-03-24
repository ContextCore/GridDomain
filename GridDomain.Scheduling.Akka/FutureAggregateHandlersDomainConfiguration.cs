using Akka.Actor;
using GridDomain.Configuration;
using Serilog;

namespace GridDomain.Scheduling.Akka {
    public class FutureAggregateHandlersDomainConfiguration : IDomainConfiguration
    {
        private readonly IActorRef _schedulingActor;
        private readonly ILogger _logger;

        public FutureAggregateHandlersDomainConfiguration(IActorRef schedulingActor, ILogger log)
        {
            _logger = log;
            _schedulingActor = schedulingActor;
        }
        public void Register(IDomainBuilder builder)
        {
            var handler = new FutureEventsSchedulingMessageHandler(_schedulingActor,_logger);
            builder.RegisterHandler<FutureEventScheduledEvent, FutureEventsSchedulingMessageHandler>(c => handler).AsSync();
            builder.RegisterHandler<FutureEventCanceledEvent, FutureEventsSchedulingMessageHandler>(c => handler).AsSync();
        }
    }
}