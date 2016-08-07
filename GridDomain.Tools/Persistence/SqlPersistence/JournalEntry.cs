namespace GridDomain.Tools.Persistence.SqlPersistence
{
  
    public class JournalEntry
    {
        public string PersistenceId { get; set; } // PersistenceId (Primary key) (length: 255)
        public long SequenceNr { get; set; } // SequenceNr (Primary key)
        public long Timestamp { get; set; } // Timestamp
        public bool IsDeleted { get; set; } // IsDeleted
        public string Manifest { get; set; } // Manifest (length: 500)
        public byte[] Payload { get; set; } // Payload
        public string Tags { get; set; } // Tags (length: 100)
    }
}