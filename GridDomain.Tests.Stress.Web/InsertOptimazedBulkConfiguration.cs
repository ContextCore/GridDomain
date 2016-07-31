using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Stress.Web
{
    public class InsertOptimazedBulkConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(30);
        public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(20);
    }
}