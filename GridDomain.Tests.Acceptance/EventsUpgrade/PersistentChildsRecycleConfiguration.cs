using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    internal class PersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
    {
        public PersistentChildsRecycleConfiguration(TimeSpan childClearPeriod, TimeSpan childMaxInactiveTime)
        {
            ChildClearPeriod = childClearPeriod;
            ChildMaxInactiveTime = childMaxInactiveTime;
        }

        public TimeSpan ChildClearPeriod { get; }
        public TimeSpan ChildMaxInactiveTime { get; }
    }
}