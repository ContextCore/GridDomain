using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaRoutes : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterSaga(SoftwareProgrammingProcess.Descriptor);
        }
    }
}