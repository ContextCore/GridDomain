using System;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class SnapshotItem
    {
        public string PersistenceId { get; set; } // PersistenceId (Primary key) (length: 255)
        public long SequenceNr { get; set; } // SequenceNr (Primary key)
        public DateTime Timestamp { get; set; } // Timestamp
        public string Manifest { get; set; } // Manifest (length: 500)
        public byte[] Snapshot { get; set; } // Snapshot
        public int? SerializerId { get; set; } // SerializerID
    }
}