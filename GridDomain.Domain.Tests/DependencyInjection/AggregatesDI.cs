using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.DI.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.DependencyInjection
{


    [TestFixture]
    public class AggregatesDI : NodeCommandsTest
    {
        [Test]
        public void Given_configured_container_When_executing_aggregate_handler_Then_container_is_available_in_aggregate_command_handler()
        {
            var testCommand = new TestCommand(42,Guid.NewGuid());
            ExecuteAndWaitFor<TestDomainEvent>(testCommand);
        }

        [Test]
        public void AggregateActor_can_be_created_with_iServiceLocator_injected()
        {
            var actorRef = Sys.ActorOf(Sys.DI().Props<AggregateActor<TestAggregate>>(),
                                       AggregateActorName.New<TestAggregate>(Guid.NewGuid()).Name);
            Assert.NotNull(actorRef);
        }

        public AggregatesDI() : base(new AutoTestAkkaConfiguration(AkkaConfiguration.LogVerbosity.Trace).ToStandAloneInMemorySystemConfig(), "TestSystem", false)
        {

        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();
            container.RegisterType<ITestDependency, TestDependencyImplementation>();
            container.RegisterInstance<IServiceLocator>(new UnityServiceLocator(container));
            container.RegisterAggregate<TestAggregate,TestAggregatesCommandHandler>();

            return new GridDomainNode(container, new TestRouteMap(), TransportMode.Standalone, Sys);
        }

    }
}
