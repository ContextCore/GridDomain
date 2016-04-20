using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public class RoutingTests:TestKit
    {
        protected ActorSystem _system;
        private AkkaPublisher _publisher;

        [TearDown]
        public void Clear()
        {
            _system.Terminate();
            _system.Dispose();
        }
        
        [SetUp]
        public void Init()
        {
            var actorSystemInfrastruture = new ActorSystemInfrastruture(new AkkaConfiguration("LocalSystem", 8100, "127.0.0.1", AkkaConfiguration.LogVerbosity.Warning));
            actorSystemInfrastruture.Init(TestActor);

            _system = actorSystemInfrastruture.System;
            _publisher = actorSystemInfrastruture.Publisher;

        }

        protected TestMessage[]  When_publishing_messages_with_same_correlation_id()
        {
            var testMessages = new InitialTestMessages().GetCommands();
            foreach (var c in testMessages)
                _publisher.Publish(c);

            return testMessages;
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