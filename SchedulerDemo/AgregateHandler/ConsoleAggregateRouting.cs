using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Scheduling.Integration;

namespace SchedulerDemo.AgregateHandler
{
    public class ConsoleAggregateRouting : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            new SchedulingRouteMap().Register(router);
            router.RegisterAggregate<ConsoleAggregate, ConsoleAggregateCommadHandler>();
        }
    }
}