using System;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
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
    }
}