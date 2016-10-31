using System;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
  
    public class JournalItem
    {
        public string PersistenceId { get; set; } // PersistenceId (Primary key) (length: 255)
        public long SequenceNr { get; set; } // SequenceNr (Primary key)
        public long Timestamp { get; set; } // Timestamp
        public bool IsDeleted { get; set; } // IsDeleted
        public string Manifest { get; set; } // Manifest (length: 500)
        public byte[] Payload { get; set; } // Payload
        public string Tags { get; set; } // Tags (length: 100)

        public JournalItem(string id, long seqNum, bool isDeleted, string manifest, DateTime timestamp,string tags, byte[] payload )
        {
            PersistenceId = id;
            SequenceNr = seqNum;
            IsDeleted = isDeleted;
            Manifest = manifest;
            Timestamp = timestamp.Ticks;
            Tags = tags;
            Payload = payload;
        }

        public JournalItem()
        {
            
        }
    }

    public class SnapshotItem
    {
        public string PersistenceId { get; set; } // PersistenceId (Primary key) (length: 255)
        public long SequenceNr { get; set; } // SequenceNr (Primary key)
        public long Timestamp { get; set; } // Timestamp datetime2(7)
        public string Manifest { get; set; } // Manifest (length: 500)
        public byte[] Snapshot { get; set; } // Payload

        public SnapshotItem(string id, long seqNum, string manifest, DateTime timestamp, byte[] snapshot)
        {
            PersistenceId = id;
            SequenceNr = seqNum;
            Manifest = manifest;
            Timestamp = timestamp.Ticks;
            Snapshot = snapshot;
        }

        public SnapshotItem()
        {

        }
    }
}