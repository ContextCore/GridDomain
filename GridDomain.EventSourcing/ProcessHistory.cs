using System.Collections.Generic;
using System.Linq;

namespace GridDomain.EventSourcing
{
    public class ProcessHistory
    {
        public IReadOnlyCollection<ProcessEntry> Steps { get; } 

        public void Add(ProcessEntry entry)
        {
            ((List<ProcessEntry>)Steps).Add(entry);
        }

        public ProcessHistory(ProcessHistory existed = null)
        {
            Steps = existed?.Steps.ToList() ?? new List<ProcessEntry>();
        }
    }
}