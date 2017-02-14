using System;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Acceptance.XUnit.Snapshots;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.SampleDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public static class TestFixtureExtensions
    {
        public static void InitFastRecycle(this NodeTestFixture fixture, TimeSpan? clearPeriod = null, TimeSpan? maxInactiveTime = null)
        {
            fixture.Add(new CustomContainerConfiguration(
                c => c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                    new PersistentChildsRecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(100),
                        maxInactiveTime ?? TimeSpan.FromMilliseconds(50)))));
        }

        public static NodeTestFixture InitSampleAggregateEachMessageSnapshots(this NodeTestFixture fixture)
        {
            fixture.Add(
                new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(
                    () => new SnapshotsPersistenceAfterEachMessagePolicy(),
                    SampleAggregate.FromSnapshot));

            return fixture;
        }
    }
}