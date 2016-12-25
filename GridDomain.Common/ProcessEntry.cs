using System;

namespace GridDomain.Common
{
    public class ProcessEntry
    {
        public ProcessEntry(string who, string what, string why = null, DateTime? @when = null)
        {
            Who = who;
            What = what;
            Why = why;
            When = when ?? BusinessDateTime.UtcNow;
        }

        public DateTime When { get; }
        public string Who { get; }
        public string Why { get; }
        public string What { get; }
    }
}