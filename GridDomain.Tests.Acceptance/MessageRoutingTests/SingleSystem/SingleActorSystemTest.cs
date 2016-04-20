using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit.NUnit;
using Google.ProtocolBuffers.Serialization;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public abstract class SingleActorSystemTest:TestKit
    {
        protected ActorSystemInfrastruture Infrastructure;

        protected TestMessage[] _initialCommands;
        protected TestMessage[] _resultMessages;

        [TearDown]
        public void Clear()
        {
            Infrastructure.System.Terminate();
            Infrastructure.System.Dispose();
        }
        
        [SetUp]
        public void Init()
        {
            Infrastructure = new ActorSystemInfrastruture(new AkkaConfiguration("LocalSystem", 8100, "127.0.0.1", AkkaConfiguration.LogVerbosity.Warning));
            Infrastructure.Init(TestActor);

            ConfigureRouter(Infrastructure.Router);

            var commands = new InitialTestMessages().GetCommands();
            Infrastructure.Publish(commands);
            _initialCommands = commands;
            _resultMessages = Infrastructure.WaitFor(this, _initialCommands.Length);
        }

        protected abstract void ConfigureRouter(ActorMessagesRouter router);
    }
}