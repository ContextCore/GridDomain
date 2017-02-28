using System.Collections.Specialized;

namespace GridDomain.Scheduling.Quartz
{
    public class InMemoryQuartzConfig : IQuartzConfig
    {
        private static int _number;

        public InMemoryQuartzConfig(string schedulerName = null)
        {
            Name = schedulerName ?? "Scheduler_" + ++ _number;
        }

        public NameValueCollection Settings
            =>
                new NameValueCollection
                {
                    ["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz",
                    ["quartz.scheduler.instanceName"] = Name
                };

        public string Name { get; }
    }
}