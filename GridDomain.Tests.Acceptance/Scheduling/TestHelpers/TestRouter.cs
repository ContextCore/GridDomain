using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestRouter : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
            await router.RegisterSaga(TestSaga.Descriptor);
        }
    }
}