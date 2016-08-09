using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    internal class TestPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => PersistentHubTestsStatus.ChildClearTime;
        public TimeSpan ChildMaxInactiveTime => PersistentHubTestsStatus.ChildMaxLifetime;
    }
}