using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
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

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();
            container.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>();
            return new GridDomainNode(container, new TestRouteMap(), TransportMode.Standalone, Sys);
        }


        [Test]
        public void Given_aggregate_When_raising_future_event_Then_it_fires_in_time()
        {
            var scheduledTime = DateTime.Now.AddSeconds(0.5);
            var msg = ExecuteAndWaitFor<TestDomainEvent>(new TestCommand(scheduledTime, Guid.NewGuid()));

            Assert.AreEqual(scheduledTime.Second, ((TestDomainEvent)msg.Message).processedTime.Second);
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
