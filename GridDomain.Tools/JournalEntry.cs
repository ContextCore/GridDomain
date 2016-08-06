using System;

namespace GridDomain.Tools
{
    public class JournalEntry
    {
        public string PersistenceId { get; set; }
        public DateTime Timestamp { get; set; }
        public int SequenceNr { get; set; }
        public bool IsDeleted { get; set; }
        public string Manifest { get; set; }
        public byte[] Payload { get; set; }
    }
}