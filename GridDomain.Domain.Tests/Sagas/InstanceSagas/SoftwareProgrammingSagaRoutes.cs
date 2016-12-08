using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaRoutes : IMessageRouteMap
    {
        public async Task Register(IMessagesRouter router)
        {
            await router.RegisterSaga(SoftwareProgrammingSaga.Descriptor);
        }
    }
}