using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Scheduling.Integration;
using SchedulerDemo.Actors;

namespace SchedulerDemo.AgregateHandler
{
    public class ConsoleAggregateRouting : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(ScheduledCommandProcessingSaga.SagaDescriptor);
            router.RegisterAggregate<ConsoleAggregate, ConsoleAggregateCommadHandler>();
        }
    }
}