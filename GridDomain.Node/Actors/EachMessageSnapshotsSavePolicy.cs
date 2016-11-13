using System;

namespace GridDomain.Node.Actors
{
    public class EachMessageSnapshotsSavePolicy : SnapshotsSavePolicy
    {
        public EachMessageSnapshotsSavePolicy():base(TimeSpan.FromSeconds(10),1)
        {
            
        }
    }

    public class NoSnapshotsSavePolicy : SnapshotsSavePolicy
    {
        public NoSnapshotsSavePolicy() : base(TimeSpan.FromDays(1000), int.MaxValue)
        {

        }
    }
}