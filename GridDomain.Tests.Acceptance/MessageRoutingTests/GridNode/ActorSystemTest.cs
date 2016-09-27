using System;
using System.Linq;
using System.Threading;
using Akka.TestKit.NUnit3;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode
{
    public abstract class ActorSystemTest<T, TInfrastructure> : TestKit where TInfrastructure : ActorSystemInfrastruture
    {
        protected T[] _resultMessages;
        public TInfrastructure Infrastructure;
        protected T[] InitialCommands;


        public ActorSystemTest():base(new AutoTestAkkaConfiguration().Copy("test_env_system",9000).ToStandAloneSystemConfig())
        {
            
        }
        [TestFixtureTearDown]
        public void Clear()
        {
            Infrastructure.Dispose();
        }

        [TestFixtureSetUp]
        public void Init()
        {
            Infrastructure = CreateInfrastructure();
            Infrastructure.Init(TestActor);
            CreateRoutes().ConfigureRouting(Infrastructure.Router);
            Thread.Sleep(500); //waiting registrations finish
            var commands = GivenCommands().GetCommands();
            Infrastructure.Publish(commands.Cast<object>().ToArray());
            InitialCommands = commands;
            _resultMessages = Infrastructure.WaitFor<T>(this, InitialCommands.Length);

            Console.WriteLine();
            Console.WriteLine("processing finished");
            Console.WriteLine();
        }

        protected override void AfterAll()
        {
            
        }

        protected abstract IGivenMessages<T> GivenCommands();
        protected abstract IRouterConfiguration CreateRoutes();
        protected abstract TInfrastructure CreateInfrastructure();
    }
}