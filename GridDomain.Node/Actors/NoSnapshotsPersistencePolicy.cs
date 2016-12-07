using System;

namespace GridDomain.Node.Actors
{
    public class NoSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public NoSnapshotsPersistencePolicy() : base(TimeSpan.FromDays(1000), int.MaxValue)
        {

        }
    }
}