using System;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.DependencyInjection.Infrastructure;
using Microsoft.Practices.Unity;
using Quartz;
using TestAggregate = GridDomain.Tests.Acceptance.Scheduling.TestHelpers.TestAggregate;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    public class SchedulerFixture : NodeTestFixture
    {
        public SchedulerFixture()
        {
            Add(new TestSagaFactoryDomainConfiguration(Logger));
            Add(new TestAggregateDomainConfiguration());
            Add(new TestRouter());

            OnNodeStartedEvent += (sender, args) => Node.Container.Resolve<IScheduler>().Clear();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            var settings = base.CreateNodeSettings();
            settings.QuartzJobRetrySettings = new InMemoryRetrySettings(4, TimeSpan.Zero);
            return settings;
        }
    }
}