using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSagaRoutemap : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterSaga(SoftwareProgrammingSaga.Descriptor);
        }
    }
}