using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit.NUnit;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{

    public class TestMessage : ICommand
    {
        public Guid CorrelationId { get; set; }
        public Guid ProcessedBy { get; }
        public Guid Id => Guid.NewGuid();
        public Guid SagaId { get; }
        public long HandlerHashCode { get; set; }
    }




    public class TestHandler : IHandler<TestMessage>
    {
        private readonly IActorRef _notifier;

        public TestHandler(IActorRef notifier)
        {
            _notifier = notifier;
        }

        public void Handle(TestMessage msg)
        {
            msg.HandlerHashCode = GetHashCode();
            _notifier.Tell(msg);
        }
    }


    [TestFixture]
    public class MessageRoutingTests:TestKit
    {
        private GridDomainNode _node;
        private ActorSystem _system;
        private AkkaPublisher _publisher;
        private ActorMessagesRouter _router;


        public void Given_correlated_routing_for_message()
        {
            _router.Route<TestMessage>()
                   .To<TestHandler>()
                   .WithCorrelation(nameof(TestMessage.CorrelationId))
                   .Register();
        }

        public void Given_not_correlated_routing_for_message()
        {
            _router.Route<TestMessage>()
                   .To<TestHandler>()
                   .Register();
        }

        [SetUp]

        private ActorMessagesRouter Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);

            GridDomainNode.ConfigureLog(autoTestGridDomainConfiguration);

            var akkaConfig = new AkkaConfiguration("LocalSystem", 8010, "127.0.0.1", "INFO");
            _system = ActorSystemFactory.CreateActorSystem(akkaConfig);
            var container = new UnityContainer();
            var propsResolver = new UnityDependencyResolver(container, _system);
            container.RegisterType<IHandler<TestMessage>, TestHandler>(new InjectionConstructor(TestActor));
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            _router = new ActorMessagesRouter(_system.ActorOf(_system.DI().Props<AkkaRoutingActor>()));
            _publisher = new AkkaPublisher(_system);
            return _router;
        }


        public TestMessage[]  When_publishing_messages_with_same_correlation_id()
        {
            var guid = Guid.NewGuid();

            var commands = new[]
            {
                new TestMessage() {CorrelationId = guid},
                new TestMessage() {CorrelationId = guid},
                new TestMessage() {CorrelationId = guid}
            };

            foreach(var c in commands)
            _publisher.Publish(c);

            return commands;
        }

        [Test]
        public void Then_It_should_be_routed_by_correlation_property()
        {
            Given_correlated_routing_for_message();
            var initialCommands = When_publishing_messages_with_same_correlation_id();

            var resultMessages = new[]
            {
                ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)),
                ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)),
                ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10))
            };

            CollectionAssert.AreEqual(initialCommands.Select(c => c.Id), resultMessages.Select(r => r.Id));

            var handlerId = resultMessages.First().HandlerHashCode;
            Assert.True(resultMessages.All(m => m.HandlerHashCode == handlerId ));
        }

        [Test]
        public void Then_It_should_not_be_routed_by_correlation_property()
        {
            Given_not_correlated_routing_for_message();
            When_publishing_messages_with_same_correlation_id();

            var hash  = ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)).HandlerHashCode;
            var hash1 = ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)).HandlerHashCode;
            var hash2 = ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)).HandlerHashCode;

            Assert.True(hash != hash1 || hash1 != hash2);
        }
    }
}
