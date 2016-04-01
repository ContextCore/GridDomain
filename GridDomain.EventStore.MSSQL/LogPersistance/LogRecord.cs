using System;
using System.ComponentModel.DataAnnotations;

namespace GridDomain.EventStore.MSSQL.LogPersistance
{
    public class LogRecord
    {
        [Key]
        public int Id { get; set; }
        public DateTime Logged { get; set; }
        public long TicksFromAppStart { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public int ThreadId { get; set; }
    }
}