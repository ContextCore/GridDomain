using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]
    public class FutureEventsTests : NodeCommandsTest
    {
        public FutureEventsTests()
            : base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "testSystem", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(200);

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {

            var config = new CustomContainerConfiguration(
                             c => c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>(),
                             c => c.RegisterType<IQuartzConfig, InMemoryQuartzConfig>());

            var node =new GridDomainNode(config, new TestRouteMap(), TransportMode.Standalone, Sys);
            return node;
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time()
        {
            var scheduledTime = DateTime.Now.AddSeconds(0.5);
            var testCommand = new TestCommand(scheduledTime, Guid.NewGuid());

            ExecuteAndWaitFor<TestDomainEvent>(testCommand);

            var aggregate = LoadAggregate<TestAggregate>(testCommand.AggregateId);
            Assert.AreEqual(scheduledTime.Second, aggregate.ProcessedTime.Second);
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_inpast_Then_it_fires_immidiatly()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_after_node_restart()
        {
            throw new NotImplementedException();
        }
    }
}
