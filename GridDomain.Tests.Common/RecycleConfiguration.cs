using System;
using GridDomain.Configuration;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.PersistentHub;

namespace GridDomain.Tests.Common
{
    public class RecycleConfiguration : IRecycleConfiguration
    {
        public RecycleConfiguration(TimeSpan childClearPeriod, TimeSpan childMaxInactiveTime)
        {
            ChildClearPeriod = childClearPeriod;
            ChildMaxInactiveTime = childMaxInactiveTime;
        }

        public TimeSpan ChildClearPeriod { get; }
        public TimeSpan ChildMaxInactiveTime { get; }
    }
}