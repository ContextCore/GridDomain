using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestRouter : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
            router.RegisterSaga(TestSaga.SagaDescriptor);
        }
    }
}