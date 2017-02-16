using System;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Acceptance.XUnit.Snapshots;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
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

        public static NodeTestFixture InitSampleAggregateSnapshots(this NodeTestFixture fixture, int keep =1 , TimeSpan? sleep=null)
        {
            fixture.Add(
                new AggregateConfiguration<SampleAggregate, SampleAggregatesCommandHandler>(
                    () => new SnapshotsPersistencePolicy(1,keep),
                    SampleAggregate.FromSnapshot));

            return fixture;
        }

        public static NodeTestFixture InitSoftwareProgrammingSagaSnapshots(this NodeTestFixture fixture, int keep = 1)
        {
            fixture.Add(
                   new CustomContainerConfiguration(
                       c =>
                           c.Register(
                               SagaConfiguration
                                   .Instance
                                   <SoftwareProgrammingSaga, SoftwareProgrammingSagaData, SoftwareProgrammingSagaFactory>(
                                       SoftwareProgrammingSaga.Descriptor,
                                       () => new SnapshotsPersistencePolicy(1, keep)))));
            return fixture;
        }
    }
}