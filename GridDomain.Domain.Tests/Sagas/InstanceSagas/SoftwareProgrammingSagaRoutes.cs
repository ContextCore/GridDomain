using System;
using System.Runtime.InteropServices;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaRoutes : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(SoftwareProgrammingSaga.Descriptor);
        }
    }

    public class SoftwareProgrammingSagaCustomRoutes : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(CustomRoutesSoftwareProgrammingSaga.Descriptor);
        }
    }
}