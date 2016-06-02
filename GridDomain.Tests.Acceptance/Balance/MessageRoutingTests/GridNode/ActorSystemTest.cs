using System;
using System.Linq;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode
{
    public abstract class ActorSystemTest<T, TInfrastructure> : TestKit where TInfrastructure : ActorSystemInfrastruture
    {
        protected T[] _resultMessages;
        public TInfrastructure Infrastructure;
        protected T[] InitialCommands;

        [TearDown]
        public void Clear()
        {
            Infrastructure.Dispose();
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

            Console.WriteLine();
            Console.WriteLine("processing finished");
            Console.WriteLine();
        }

        protected abstract IGivenCommands<T> GivenCommands();

        protected abstract IRouterConfiguration CreateRoutes();
        protected abstract TInfrastructure CreateInfrastructure();
    }
}