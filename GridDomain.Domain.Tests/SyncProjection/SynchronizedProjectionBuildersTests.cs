using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using GridDomain.Tests.SyncProjection.SampleDomain;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.SyncProjection
{
    [TestFixture]
    class SynchronizedProjectionBuildersTests : NodeCommandsTest
    {
        private ExpectedMessagesRecieved _ProcessedEvents;

        public SynchronizedProjectionBuildersTests():
            base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "ProjectionBuilders", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromMinutes(1);
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container  = new UnityContainer();
            CompositionRoot.Init(container,Sys, dbConfig, TransportMode.Standalone);
            container.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>();

            return new GridDomainNode(container, 
                                      new TestRouteMap(new UnityServiceLocator(container)), 
                                      TransportMode.Standalone,Sys);
        }

        [TestFixtureSetUp]
        public void When_execute_many_commands_for_create_and_update()
        {
            var createCommands = Enumerable.Range(0, 10).Select(r => new CreateAggregateCommand(101, Guid.NewGuid())).ToArray();
            var aggregateIds = createCommands.Select(c => c.AggregateId).ToArray();
            var updateCommands = Enumerable.Range(0, 20).Select(r => new CreateAggregateCommand(101, aggregateIds.RandomElement())).ToArray();

            var allCommands = createCommands.Concat(updateCommands).ToArray();

            _ProcessedEvents = ExecuteAndWaitForMany<AggregateCreatedEvent, AggregateChangedEvent>(createCommands.Length,
                                                                                                   updateCommands.Length,
                                                                                                   allCommands);
        }
        [Test]
        public void All_changed_events_should_be_processed_in_commad_ordering()
        {
            int a = 1;
        }

    }
}
