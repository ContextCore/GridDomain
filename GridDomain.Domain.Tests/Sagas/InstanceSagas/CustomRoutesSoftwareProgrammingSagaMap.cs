using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class CustomRoutesSoftwareProgrammingSagaMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(CustomRoutesSoftwareProgrammingSaga.Descriptor);
        }
    }
}