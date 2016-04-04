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
        private GridDomainNode _node;

        class TestMessage:ICommand
        {
            public Guid CorrelationId;
            public Guid ProcessedBy;
            public Guid Id { get; }
            public Guid SagaId { get; }
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
            private readonly IActorRef _notifier;

            public TestHandler(Guid id,IActorRef notifier)
            {
                _notifier = notifier;
                _id = id;
            }

            public void Handle(TestMessage msg)
            {
                msg.ProcessedBy = _id;
                _notifier.Tell(msg);
            }
        }

        [SetUp]
        public void Given_not_correlated_routing_for_message()
        {
            var akkaConfig = new AkkaConfiguration("LocalSystem", 8000, "127.0.0.1", "ERROR");
            var system = ActorSystemFactory.CreateActorSystem(akkaConfig);
            var container = new UnityContainer();

            var guid1 = Guid.NewGuid();
            container.RegisterType<IHandler<TestMessage>, TestHandler>("handler1",new InjectionConstructor(guid1,TestActor));

            var guid2 = Guid.NewGuid();
            container.RegisterType<IHandler<TestMessage>, TestHandler>("handler2", new InjectionConstructor(guid2, TestActor));


            var router = new ActorMessagesRouter(system);
            router.Route<TestMessage>()
                  .To<TestHandler>()
                //  .WithCorrelation(nameof(TestMessage.CorrelationId))
                  .Register();

            _node = new GridDomainNode(container,system);
        }
        

        [Test]
        public void Then_publishing_message_It_should_be_routed_by_correlation_property()
        {
          _node.Execute(new );  
        }
    }
}
