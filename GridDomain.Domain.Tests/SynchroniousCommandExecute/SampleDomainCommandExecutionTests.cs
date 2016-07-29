using System;
using System.Diagnostics;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.SynchroniousCommandExecute
{

    public class SampleDomainContainerConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterAggregate<SampleAggregate, SampleAggregatesCommandHandler>();
            container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            container.RegisterType<AggregateCreatedProjectionBuilder>();
            container.RegisterType<SampleProjectionBuilder>();
        }
    }
    public class SampleDomainCommandExecutionTests : ExtendedNodeCommandTest
    {
        protected override TimeSpan Timeout => Debugger.IsAttached
            ? TimeSpan.FromMinutes(10)
            : TimeSpan.FromSeconds(5);

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new SampleDomainContainerConfiguration();
        }

        protected override IMessageRouteMap CreateMap()
        {
            var container = new UnityContainer();
            container.Register(CreateConfiguration());
            return new SampleRouteMap(container);

        }
        
        public SampleDomainCommandExecutionTests(bool inMemory) : base(inMemory)
        {
        }
    }
}