using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.XUnit.DependencyInjection.Infrastructure
{
    internal class TestRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterAggregate(TestAggregatesCommandHandler.Descriptor);
        }
    }
}