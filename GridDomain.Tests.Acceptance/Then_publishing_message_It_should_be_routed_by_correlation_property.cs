using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    [TestFixture]
    public class MessageRoutingTests
    {
   
        [SetUp]
        public void Given_correlated_routing_for_message()
        {
            var akkaConfig = new AkkaConfiguration("LocalSystem", 8000, "127.0.0.1", "ERROR");
            var system = GridDomainNode.CreateActorSystem(akkaConfig);
            var router = new ActorMessagesRouter(system);
           // router.Route<Bala>()
        }
        

        [Test]
        public void Then_publishing_message_It_should_be_routed_by_correlation_property()
        {
            
        }
    }
}
