using System;
using System.Collections.Generic;
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

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public class RoutingTests:TestKit
    {
        protected ActorSystem _system;
        private AkkaPublisher _publisher;
        protected ActorMessagesRouter Router;
        protected AkkaConfiguration _akkaConfig;


        public class TestMessage : ICommand
        {
            public Guid CorrelationId { get; set; }
            public Guid ProcessedBy { get; }
            public Guid Id { get; } = Guid.NewGuid();
            public Guid SagaId { get; }
            public long HandlerHashCode { get; set; }

            public int HandleOrder { get; set; }
            public int ExecuteOrder { get; set; }
        }


        public class TestHandler : IHandler<TestMessage>
        {
            private readonly IActorRef _notifier;
            private int _handleCounter = 0;

            public TestHandler(IActorRef notifier)
            {
                _notifier = notifier;
            }

            public void Handle(TestMessage e)
            {
                e.HandlerHashCode = GetHashCode();
                e.HandleOrder = ++_handleCounter;
                _notifier.Tell(e);
            }
        }

        [TearDown]
        public void Clear()
        {
            _system.Terminate();
            _system.Dispose();
        }


        protected virtual void InitAkkaSystem()
        {
            _system = ActorSystemFactory.CreateActorSystem(_akkaConfig);
        }

        [SetUp]
        public void Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);
            GridDomainNode.ConfigureLog(autoTestGridDomainConfiguration);


            _akkaConfig = new AkkaConfiguration("LocalSystem", 8021, "127.0.0.1", AkkaConfiguration.LogVerbosity.Warning);
            InitAkkaSystem();
            var container = new UnityContainer();
            var propsResolver = new UnityDependencyResolver(container, _system);
            InitContainer(container);
            Router = new ActorMessagesRouter(_system.ActorOf(_system.DI().Props<AkkaRoutingActor>()));
            _publisher = new AkkaPublisher(_system);
        }

        protected virtual void InitContainer(UnityContainer container)
        {
            container.RegisterType<IHandler<TestMessage>, TestHandler>(new InjectionConstructor(TestActor));
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
        }


        protected TestMessage[]  When_publishing_messages_with_same_correlation_id()
        {
            var guid = Guid.NewGuid();
            var guid1 = Guid.NewGuid();
            int count = 0;
            int count1 = 0;

        
            var commands = new[]
            {
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage() {CorrelationId = guid, ExecuteOrder = ++count}
            };

            foreach (var c in commands)
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
    }
}