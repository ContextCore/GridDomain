using System;

namespace GridDomain.Node.Actors
{
    public class EachMessageSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public EachMessageSnapshotsPersistencePolicy():base(1)
        {
            
        }
    }
}