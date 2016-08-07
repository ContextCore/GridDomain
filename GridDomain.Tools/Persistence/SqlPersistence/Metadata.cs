namespace GridDomain.Tools.SqlPersistence
{
  
    public class Metadata
    {
        public string PersistenceId { get; set; } // PersistenceId (Primary key) (length: 255)
        public long SequenceNr { get; set; } // SequenceNr (Primary key)
    }
}