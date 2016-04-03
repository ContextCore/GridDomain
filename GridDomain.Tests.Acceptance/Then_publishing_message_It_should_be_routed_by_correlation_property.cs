using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    [TestFixture]
    public class MessageRoutingTests:TestKit
    {

        class TestMessage
        {
            public Guid Id;
            public Guid CorrelationId;
            public Guid ProcessedBy;
        }

        class TestHandlerActor:ReceiveActor
        {
            public TestHandlerActor(IActorRef notifyActor, Guid id)
            {
                Receive<TestMessage>(m =>
                {
                    m.ProcessedBy = id;
                    notifyActor.Tell(m);
                });
            }
        }

        class TestHandler : IHandler<TestMessage>
        {
            private readonly Guid _id;

            public TestHandler(Guid id)
            {
                _id = id;
            }

            public void Handle(TestMessage msg)
            {
                msg.ProcessedBy = _id;
            }
        }

        [SetUp]
        public void Given_correlated_routing_for_message()
        {
            var akkaConfig = new AkkaConfiguration("LocalSystem", 8000, "127.0.0.1", "ERROR");
            var system = ActorSystemFactory.CreateActorSystem(akkaConfig);
            var container = new UnityContainer();
            container.RegisterType<IHandler<TestMessage>, TestHandler>();

            var router = new ActorMessagesRouter(system);
            router.Route<TestMessage>()
                  .To<TestHandler>()
                  .WithCorrelation(nameof(TestMessage.CorrelationId))
                  .Register();

            var node = new GridDomainNode(container,system);
        }
        

        [Test]
        public void Then_publishing_message_It_should_be_routed_by_correlation_property()
        {
            
        }
    }
}
