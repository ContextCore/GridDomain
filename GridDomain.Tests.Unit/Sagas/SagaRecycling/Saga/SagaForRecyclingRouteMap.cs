using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Unit.Sagas.SagaRecycling.Saga
{
    public class SagaForRecyclingRouteMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterSaga(SagaForRecycling.Descriptor);
        }
    }
}