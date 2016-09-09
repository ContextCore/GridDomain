namespace GridDomain.Node.MessageDump
{
    public class JournalEntryFault
    {
        public string SourceId { get; set; }
        public long Timestamp { get; set; }
        public string Manifest { get; set; }
        public string Payload { get; set; }

        public static JournalEntryFault FromEntry(DebugJournalEntry entry)
        {
            return new JournalEntryFault()
            {
                Manifest = entry.Manifest,
                SourceId = entry.SourceId,
                Timestamp = entry.Timestamp,
                Payload = entry.Payload
            };
        }
    }
}