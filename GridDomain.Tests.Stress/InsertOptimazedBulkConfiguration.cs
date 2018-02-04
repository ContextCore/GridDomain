using System;
using GridDomain.Configuration;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.PersistentHub;

namespace GridDomain.Tests.Stress
{
    public class InsertOptimazedBulkConfiguration : IRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(30);
        public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(30);
    }
}