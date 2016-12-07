using System;

namespace GridDomain.Node.Actors
{
    public class EachMessageSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public EachMessageSnapshotsPersistencePolicy():base(TimeSpan.FromSeconds(10),1)
        {
            
        }
    }
}