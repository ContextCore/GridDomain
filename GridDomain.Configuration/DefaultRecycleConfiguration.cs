using System;

namespace GridDomain.Configuration
{
    public class DefaultRecycleConfiguration : IRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod { get; } = TimeSpan.FromMinutes(5);
        public TimeSpan ChildMaxInactiveTime { get; } = TimeSpan.FromMinutes(10);
    }
}