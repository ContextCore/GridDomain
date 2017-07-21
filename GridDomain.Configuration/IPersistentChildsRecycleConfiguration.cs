using System;

namespace GridDomain.Configuration
{
    public interface IPersistentChildsRecycleConfiguration
    {
        TimeSpan ChildClearPeriod { get; }
        TimeSpan ChildMaxInactiveTime { get; }
    }
}