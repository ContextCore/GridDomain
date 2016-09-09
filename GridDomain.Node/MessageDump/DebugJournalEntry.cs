using System.ComponentModel.DataAnnotations;

namespace GridDomain.Node.MessageDump
{
    public class DebugJournalEntry 
    {
        [Key]
        public string SourceId { get; set; } 
        // public int 

        public long Timestamp { get; set; }
        public string Manifest { get; set; } 
        public string Payload { get; set; } 
    }
}