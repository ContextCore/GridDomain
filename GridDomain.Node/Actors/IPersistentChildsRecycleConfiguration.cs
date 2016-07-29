using System;

namespace GridDomain.Node.Actors
{
    public interface IPersistentChildsRecycleConfiguration
    {
        TimeSpan ChildClearPeriod { get; }
        TimeSpan ChildMaxInactiveTime { get; }
    }
}