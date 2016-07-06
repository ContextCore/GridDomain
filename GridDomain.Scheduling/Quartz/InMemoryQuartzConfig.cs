using System.Collections.Specialized;

namespace GridDomain.Scheduling.Quartz
{
    public class InMemoryQuartzConfig : IQuartzConfig
    {
        public string StorageType => "Quartz.Simpl.RAMJobStore, Quartz";
        public NameValueCollection Settings => new NameValueCollection
        {
            ["quartz.jobStore.type"] = StorageType,
        };
    }
}