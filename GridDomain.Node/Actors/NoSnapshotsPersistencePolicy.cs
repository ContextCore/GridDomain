using System;

namespace GridDomain.Node.Actors
{
    public class NoSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public NoSnapshotsPersistencePolicy() : base(int.MaxValue)
        {

        }
    }
}