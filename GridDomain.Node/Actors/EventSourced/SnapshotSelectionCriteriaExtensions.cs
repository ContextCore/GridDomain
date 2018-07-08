using GridDomain.Configuration.SnapshotPolicies;

namespace GridDomain.Node.Actors.EventSourced {
    public static class SnapshotSelectionCriteriaExtensions
    {
        public static SnapshotSelectionCriteria ToGridDomain(this Akka.Persistence.SnapshotSelectionCriteria c)
        {
            return new SnapshotSelectionCriteria(c.MaxSequenceNr, c.MaxTimeStamp, c.MinSequenceNr, c.MinTimestamp);

        }
        public static Akka.Persistence.SnapshotSelectionCriteria ToGridDomain(this SnapshotSelectionCriteria c)
        {
            return new Akka.Persistence.SnapshotSelectionCriteria(c.MaxSequenceNr, c.MaxTimeStamp, c.MinSequenceNr, c.MinTimestamp);
        }

    }
}