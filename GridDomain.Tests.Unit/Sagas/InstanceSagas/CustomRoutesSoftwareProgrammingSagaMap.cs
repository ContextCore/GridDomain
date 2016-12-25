using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    public class CustomRoutesSoftwareProgrammingSagaMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterSaga(CustomRoutesSoftwareProgrammingSaga.Descriptor);
        }
    }
}