using System.Threading;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Tests.Unit.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Unit
{
    [TestFixture]
    public class Given_Node_When_Start_TrasportRegistrations_Test : InMemorySampleDomainTests
    {
        [Then]
        public void Transport_contains_all_registrations()
        {
            var transport = (LocalAkkaEventBusTransport) GridNode.Transport;
            Thread.Sleep(1000);
            CollectionAssert.IsNotEmpty(transport.Subscribers);
        }
    }
}