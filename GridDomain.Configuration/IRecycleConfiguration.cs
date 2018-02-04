using System;

namespace GridDomain.Configuration
{
    public interface IRecycleConfiguration
    {
        TimeSpan ChildClearPeriod { get; }
        TimeSpan ChildMaxInactiveTime { get; }
    }
}