using System;

namespace GridDomain.Node.Actors.PersistentHub
{
    public interface IPersistentChildsRecycleConfiguration
    {
        TimeSpan ChildClearPeriod { get; }
        TimeSpan ChildMaxInactiveTime { get; }
    }
}