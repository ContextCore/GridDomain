using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Sagas.SagaRecycling.Saga
{
    public class SagaForRecyclingRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(SagaForRecycling.Descriptor);
        }
    }
}