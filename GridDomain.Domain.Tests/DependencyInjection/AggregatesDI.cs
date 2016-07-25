using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.DependencyInjection.Infrastructure;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.DependencyInjection
{
    public class AggregatesDI : NodeCommandsTest
    {

        public AggregatesDI() : base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "TestSystem", false)
        {

        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();

            container.RegisterType<ITestDependency, TestDependencyImplementation>();
          //  container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            container.RegisterAggregate<TestAggregate,TestAggregatesCommandHandler>();

            return new GridDomainNode(container, new TestRouteMap(), TransportMode.Standalone, Sys);
        }

    }
}
