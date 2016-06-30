using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using GridDomain.Tests.SyncProjection.SampleDomain;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests.SyncProjection
{
    [TestFixture]
    class SynchronizedProjectionBuildersTests : NodeCommandsTest
    {
        public SynchronizedProjectionBuildersTests(string config, string name = null, bool clearDataOnStart = true) : base(config, name, clearDataOnStart)
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

        [Test]
        public void All_changed_event_should_be_processed_after_created()
        {
            
        }
    }
}
