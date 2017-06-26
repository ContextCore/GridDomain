using System;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Unit;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    public class SchedulerFixture : NodeTestFixture
    {
        public SchedulerFixture()
        {
            Add(new SchedulerContainerConfiguration());
            Add(new TestRouter());

            OnNodeStartedEvent += (sender, args) => Node.Container.Resolve<IScheduler>().Clear();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            var settings = base.CreateNodeSettings();
            settings.QuartzJobRetrySettings = new InMemoryRetrySettings(4, TimeSpan.Zero);
            return settings;
        }

        private class SchedulerContainerConfiguration : IContainerConfiguration
        {
            public void Register(IUnityContainer container)
            {
                container.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
                container.Register(SagaConfiguration.New<TestSaga, TestSagaState, TestSagaFactory>(TestSaga.Descriptor));
            }
        }
    }
}