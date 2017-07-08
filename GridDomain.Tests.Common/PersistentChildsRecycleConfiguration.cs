using System;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.PersistentHub;

namespace GridDomain.Tests.Common
{
    public class PersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
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