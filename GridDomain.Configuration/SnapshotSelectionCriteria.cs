using System;

namespace GridDomain.Configuration {
    public class SnapshotSelectionCriteria
    {
        public long MaxSequenceNr;
        public DateTime MaxTimeStamp;
        public long MinSequenceNr;
        public DateTime? MinTimestamp;
    }
}