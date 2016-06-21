using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Scheduling.Integration
{
    public class SchedulingRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(ScheduledCommandProcessingSaga.SagaDescriptor);
        }
    }
}
