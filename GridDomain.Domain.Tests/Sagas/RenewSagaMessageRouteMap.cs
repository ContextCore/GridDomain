using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Sagas
{
    public class RenewSagaMessageRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(SubscriptionRenewSaga.SubscriptionRenewSaga.Descriptor);
        }
    }
}