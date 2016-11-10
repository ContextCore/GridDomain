using System;

namespace GridDomain.Node.Actors
{
    public class DefaultSnapshotsSavePolicy : SnapshotsSavePolicy
    {
        public DefaultSnapshotsSavePolicy():base(TimeSpan.FromSeconds(10),1)
        {
            
        }
    }
}