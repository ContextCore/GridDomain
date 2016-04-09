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

    //[TestFixture]
    //public class MessageRoutingTest_Node : NodeCommandsTest
    //{

    //    [SetUp]
    //    public void Init()
    //    {
    //        GridNode.Container.RegisterType<IHandler<MessageRoutingTests.TestMessage>, MessageRoutingTests.TestHandler>(new InjectionConstructor(TestActor));
    //        GridNode.Container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();

    //        var router = new ActorMessagesRouter(GridNode.System.ActorOf(GridNode.System.DI().Props<AkkaRoutingActor>()));
    //        router.Route<MessageRoutingTests.TestMessage>()
    //              .To<MessageRoutingTests.TestHandler>()
    //              .WithCorrelation(nameof(MessageRoutingTests.TestMessage.CorrelationId))
    //              .Register();
    //    }


    //    [Test]
    //    public void Then_It_should_be_routed_by_correlation_property()
    //    {
    //        var guid = Guid.NewGuid();

    //        var cmds = new MessageRoutingTests.TestMessage[]
    //        {
    //            new MessageRoutingTests.TestMessage() {CorrelationId = guid},
    //            new MessageRoutingTests.TestMessage() {CorrelationId = guid},
    //            new MessageRoutingTests.TestMessage() {CorrelationId = guid}
    //        };
    //        ExecuteAndWaitFor<MessageRoutingTests.TestMessage>(cmds, c => c.Id);
    //        Assert.True(hash == hash1 && hash1 == hash2);
    //    }

    //}


    public class TestMessage : ICommand
    {
        public Guid CorrelationId { get; set; }
        public Guid ProcessedBy { get; }
        public Guid Id { get; }
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

 
        [SetUp]
        public void Given_correlated_routing_for_message()
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
            var router = new ActorMessagesRouter(_system.ActorOf(_system.DI().Props<AkkaRoutingActor>()));

            router.Route<TestMessage>()
                  .To<TestHandler>()
                  .WithCorrelation(nameof(TestMessage.CorrelationId))
                  .Register();

            _publisher = new AkkaPublisher(_system);
        }
        

        public void When_publishing_messages_with_same_correlation_id()
        {
            var guid = Guid.NewGuid();

            _publisher.Publish(new TestMessage() { CorrelationId = guid });
            _publisher.Publish(new TestMessage() { CorrelationId = guid });
            _publisher.Publish(new TestMessage() { CorrelationId = guid });
        }

        [Test]
        public void Then_It_should_be_routed_by_correlation_property()
        {
            When_publishing_messages_with_same_correlation_id();

            var hash  = ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)).HandlerHashCode;
            var hash1 = ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)).HandlerHashCode;
            var hash2 = ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)).HandlerHashCode;

            Assert.True(hash == hash1 && hash1 == hash2);
        }
    }
}
