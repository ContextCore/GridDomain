using System;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node;
using Serilog;

namespace GridDomain.Scheduling.Akka {
    public class FutureAggregateHandlersDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterNodeHandler<FutureEventScheduledEvent, FutureEventsSchedulingMessageHandler>(c => 
                 new FutureEventsSchedulingMessageHandler(c.System.GetExtension<SchedulingExtension>().SchedulingActor,c.Log))
                   .AsSync();
            
            builder.RegisterNodeHandler<FutureEventCanceledEvent,  FutureEventsSchedulingMessageHandler>(c => 
                    new FutureEventsSchedulingMessageHandler(c.System.GetExtension<SchedulingExtension>().SchedulingActor,c.Log)).AsSync();
        }
    }
}