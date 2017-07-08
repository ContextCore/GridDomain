using System;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.PersistentHub;

namespace GridGomain.Tests.Stress
{
    public class InsertOptimazedBulkConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(30);
        public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(30);
    }
}