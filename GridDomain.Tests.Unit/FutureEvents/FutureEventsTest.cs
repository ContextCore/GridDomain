using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public abstract class FutureEventsTest : ExtendedNodeCommandTest
    {
        protected IScheduler Scheduler;

        protected FutureEventsTest(bool inMemory) : base(inMemory)
        {

        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>(),
                c => c.RegisterInstance(CreateQuartzConfig()));
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new TestRouteMap();
        }


        protected override async Task Start()
        {
            await base.Start();
            Scheduler = GridNode.Container.Resolve<IScheduler>();
            Scheduler.Clear();
        }

        protected virtual IQuartzConfig CreateQuartzConfig()
        {
            return InMemory ? (IQuartzConfig) new InMemoryQuartzConfig() : new PersistedQuartzConfig();
        }

        protected async Task<TestAggregate> RaiseFutureEventInTime(DateTime scheduledTime)
        {
            var testCommand = new ScheduleEventInFutureCommand(scheduledTime, Guid.NewGuid(), "test value");

            await GridNode.Prepare(testCommand).Expect<TestDomainEvent>().Execute();

            return LoadAggregate<TestAggregate>(testCommand.AggregateId);
        }
    }
}