using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.TestKit.NUnit;
using Google.ProtocolBuffers.Serialization;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public abstract class ActorSystemTest<T, TInfrastructure>:TestKit where TInfrastructure : ActorSystemInfrastruture
    {
        public TInfrastructure Infrastructure;
        protected T[] InitialCommands;
        protected T[] _resultMessages;

        [TearDown]
        public void Clear()
        {
            Infrastructure.System.Terminate();
            Infrastructure.System.Dispose();
        }
        
        [SetUp]
        public void Init()
        {
            Infrastructure = CreateInfrastructure();
            Infrastructure.Init(TestActor);

            var commands = GivenCommands().GetCommands();
            CreateRoutes().ConfigureRouting(Infrastructure.Router);

            Infrastructure.Publish(commands.Cast<object>().ToArray());
            InitialCommands = commands;
            _resultMessages = Infrastructure.WaitFor<T>(this, InitialCommands.Length);
        }

        protected abstract IGivenCommands<T> GivenCommands();

        protected abstract IRouterConfiguration CreateRoutes();
        protected abstract TInfrastructure CreateInfrastructure();

    }
}