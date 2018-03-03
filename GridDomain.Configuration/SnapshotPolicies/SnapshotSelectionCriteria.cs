using System;

namespace GridDomain.Configuration {
    public class SnapshotSelectionCriteria
    {
        public long MaxSequenceNr { get; }
        public DateTime MaxTimeStamp { get; }
        public long MinSequenceNr { get; }
        public DateTime? MinTimestamp { get; }

        public static SnapshotSelectionCriteria Empty { get; } = new SnapshotSelectionCriteria(0);
        public SnapshotSelectionCriteria(long maxSequenceNr, DateTime? maxTimeStamp=null, long minSequenceNr = 0, DateTime? minTimestamp = null)
        {
            MaxSequenceNr = maxSequenceNr;
            MaxTimeStamp = maxTimeStamp ?? DateTime.MaxValue;
            MinSequenceNr = minSequenceNr;
            MinTimestamp = minTimestamp;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SnapshotSelectionCriteria) obj);
        }

        protected bool Equals(SnapshotSelectionCriteria other)
        {
            return MaxSequenceNr == other.MaxSequenceNr
                   && MaxTimeStamp.Equals(other.MaxTimeStamp)
                   && MinSequenceNr == other.MinSequenceNr
                   && MinTimestamp.Equals(other.MinTimestamp);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MaxSequenceNr.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxTimeStamp.GetHashCode();
                hashCode = (hashCode * 397) ^ MinSequenceNr.GetHashCode();
                hashCode = (hashCode * 397) ^ MinTimestamp.GetHashCode();
                return hashCode;
            }
        }
    }
}