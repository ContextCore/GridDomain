using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using Microsoft.Practices.Unity;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling
{
    public class SchedulerFixture : NodeTestFixture
    {
        public SchedulerFixture()
        {
            Add(new SchedulerContainerConfiguration());
            Add(new TestRouter());
        }

        protected override NodeSettings CreateNodeSettings()
        {
            var settings = base.CreateNodeSettings();
            settings.QuartzJobRetrySettings = new InMemoryRetrySettings(4, TimeSpan.Zero);
            return settings;
        }

        protected override void OnNodeStarted()
        {
            var scheduler = Node.Container.Resolve<Quartz.IScheduler>();
            scheduler.Clear();
        }

        class SchedulerContainerConfiguration : IContainerConfiguration
        {
            public void Register(IUnityContainer container)
            {
                container.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
                container.Register(new SagaConfiguration<TestSaga, TestSagaState, TestSagaFactory>(TestSaga.Descriptor));
            }
        }
    }
}