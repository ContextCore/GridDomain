using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaCustomRoutes : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(CustomRoutesSoftwareProgrammingSaga.Descriptor);
        }
    }
}