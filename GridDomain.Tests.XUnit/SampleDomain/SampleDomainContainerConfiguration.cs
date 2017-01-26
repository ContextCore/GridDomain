using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.XUnit.SampleDomain
{
    public class SampleDomainContainerConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterAggregate<SampleAggregate, SampleAggregatesCommandHandler>();
            container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            container.RegisterType<AggregateCreatedProjectionBuilder>();
            container.RegisterType<SampleProjectionBuilder>();
            container.RegisterType<FaultyCreateProjectionBuilder>();
        }
    }
}