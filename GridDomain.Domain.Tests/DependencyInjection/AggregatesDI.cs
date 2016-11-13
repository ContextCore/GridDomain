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
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.DependencyInjection.Infrastructure;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.DependencyInjection
{
    public class AggregatesDI : NodeCommandsTest
    {
        private static readonly AutoTestAkkaConfiguration Config = new AutoTestAkkaConfiguration();

        public AggregatesDI() : base(Config.ToStandAloneInMemorySystemConfig(),
                                     Config.Network.SystemName,
                                     false)
        {

        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {

            var conf = new CustomContainerConfiguration(c =>
            {
                c.RegisterType<ITestDependency, TestDependencyImplementation>();
                c.RegisterInstance<IUnityContainer>(c);
                c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
                c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>();
            });
            return new GridDomainNode(conf, new TestRouteMap(), () => Sys);
        }

    }
}
