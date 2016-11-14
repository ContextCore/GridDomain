using System;
using GridDomain.Node.Actors;

namespace GridGomain.Tests.Stress
{
    public class InsertOptimazedBulkConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(1);
        public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(1);
    }
}