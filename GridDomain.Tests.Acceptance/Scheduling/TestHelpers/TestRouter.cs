using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Scheduling.Integration;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestRouter : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
            router.RegisterSaga(ScheduledCommandProcessingSaga.SagaDescriptor);
        }
    }
}