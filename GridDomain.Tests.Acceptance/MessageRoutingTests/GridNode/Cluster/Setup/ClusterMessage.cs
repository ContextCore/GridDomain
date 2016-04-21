using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS;
using GridDomain.Node;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class ClusterMessage : TestMessage
    {
        public Address ProcessorActorSystemAdress { get; set; }
    }
}
