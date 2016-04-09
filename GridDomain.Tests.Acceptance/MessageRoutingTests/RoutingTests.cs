using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
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
    public class RoutingTests:TestKit
    {
        private GridDomainNode _node;
        private ActorSystem _system;
        private AkkaPublisher _publisher;
        protected ActorMessagesRouter _router;


        public class TestMessage : ICommand
        {
            public Guid CorrelationId { get; set; }
            public Guid ProcessedBy { get; }
            public Guid Id { get; } = Guid.NewGuid();
            public Guid SagaId { get; }
            public long HandlerHashCode { get; set; }

            public int HandlerThreadId { get; set; }
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
                msg.HandlerThreadId = Thread.CurrentThread.ManagedThreadId;
                _notifier.Tell(msg);
            }
        }


        private void Given_not_correlated_routing_for_message(ActorMessagesRouter actorMessagesRouter)
        {
            actorMessagesRouter.Route<TestMessage>()
                .To<TestHandler>()
                .Register();
        }

        [TearDown]
        public void Clear()
        {
            _system.Terminate();
            _system.Dispose();
        }

        [SetUp]
        protected ActorMessagesRouter Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);

            GridDomainNode.ConfigureLog(autoTestGridDomainConfiguration);

            var akkaConfig = new AkkaConfiguration("LocalSystem", 8020, "127.0.0.1", "INFO");
            _system = ActorSystemFactory.CreateActorSystem(akkaConfig);
            var container = new UnityContainer();
            var propsResolver = new UnityDependencyResolver(container, _system);
            container.RegisterType<IHandler<TestMessage>, TestHandler>(new InjectionConstructor(TestActor));
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            _router = new ActorMessagesRouter(_system.ActorOf(_system.DI().Props<AkkaRoutingActor>()));
            _publisher = new AkkaPublisher(_system);

            return _router;
        }


        protected TestMessage[]  When_publishing_messages_with_same_correlation_id()
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

  

        protected TestMessage[] WaitFor(int number)
        {
            var resultMessages = new List<TestMessage>();
            for(int num = 0; num < number; num++)
                resultMessages.Add(ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)));

            return resultMessages.ToArray();
        }

        [Test]
        public void Then_It_should_not_be_routed_by_correlation_property()
        {
            Given_not_correlated_routing_for_message(_router);
            var initialCommands = When_publishing_messages_with_same_correlation_id();

            var resultMessages = new[]
            {
                ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)),
                ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)),
                ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10))
            };

            CollectionAssert.AreEquivalent(initialCommands.Select(c => c.Id), resultMessages.Select(r => r.Id));
            var handlerId = resultMessages.First().HandlerHashCode;
            var threadId = resultMessages.First().HandlerThreadId;

            Assert.True(resultMessages.Any(m => m.HandlerHashCode != handlerId));
            Assert.True(resultMessages.Any(m => m.HandlerThreadId != threadId));
        }
    }
}