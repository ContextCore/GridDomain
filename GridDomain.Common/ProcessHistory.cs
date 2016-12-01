using System.Collections.Generic;
using System.Linq;

namespace GridDomain.Common
{
    public class ProcessHistory
    {
        public IReadOnlyCollection<ProcessEntry> Steps { get; } 

        public void Add(ProcessEntry entry)
        {
            ((List<ProcessEntry>)Steps).Add(entry);
        }

        public ProcessHistory(params ProcessEntry[] existed)
        {
            Steps = new List<ProcessEntry>(existed);
        }
    }
}