using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.XUnit.SampleDomain;

namespace GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain
{
    public class CustomRoutesSoftwareProgrammingSagaMap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterSaga(CustomRoutesSoftwareProgrammingSaga.Descriptor);
        }
    }
}