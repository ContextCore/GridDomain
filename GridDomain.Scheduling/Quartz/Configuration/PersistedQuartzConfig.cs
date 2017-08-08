using System.Collections.Specialized;
using System.Configuration;
using GridDomain.Scheduling.Quartz.Retry;

namespace GridDomain.Scheduling.Quartz.Configuration
{
    public class PersistedQuartzConfig : IQuartzConfig
    {
        private const string QuartzConnectionStringName = "Quartz";

        public string ConnectionString
            =>
                ConfigurationManager.ConnectionStrings[QuartzConnectionStringName]?.ConnectionString
                ?? "Server=(local); Database = Quartz; Integrated Security = true; MultipleActiveResultSets = True";

        public string StorageType => "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";

        public NameValueCollection Settings
            =>
                new NameValueCollection
                {
                    ["quartz.jobStore.type"] = StorageType,
                    ["quartz.jobStore.clustered"] = "true",
                    ["quartz.jobStore.dataSource"] = "default",
                    ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                    ["quartz.jobStore.lockHandler.type"] =
                    "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                    ["quartz.dataSource.default.connectionString"] = ConnectionString,
                    ["quartz.dataSource.default.provider"] = "SqlServer-20",
                    ["quartz.scheduler.instanceId"] = "AUTO"
                };

        public string Name { get; }
        public IRetrySettings RetryOptions { get; set; } = new InMemoryRetrySettings();
    }
}