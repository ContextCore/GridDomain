using System;
using System.Runtime.InteropServices;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaRoutes : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga<SoftwareProgrammingSaga,SoftwareProgrammingSagaData>();
        }
    }
}