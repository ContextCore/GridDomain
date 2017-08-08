namespace GridDomain.Node.Actors.EventSourced
{
    public class EachMessageSnapshotsPersistencePolicy : SnapshotsPersistencePolicy
    {
        public EachMessageSnapshotsPersistencePolicy() : base(1) {}
    }
}