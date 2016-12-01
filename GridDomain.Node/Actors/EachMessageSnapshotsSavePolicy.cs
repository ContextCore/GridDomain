using System;

namespace GridDomain.Node.Actors
{
    public class EachMessageSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public EachMessageSnapshotsPersistencePolicy():base(TimeSpan.FromSeconds(10),1)
        {
            
        }
    }

    public class NoSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public NoSnapshotsPersistencePolicy() : base(TimeSpan.FromDays(1000), int.MaxValue)
        {

        }
    }
}