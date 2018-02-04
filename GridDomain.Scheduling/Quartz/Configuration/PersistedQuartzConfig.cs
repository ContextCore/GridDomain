using System;
using System.Collections.Specialized;
using GridDomain.Scheduling.Quartz.Retry;

namespace GridDomain.Scheduling.Quartz.Configuration
{
    public class PersistedQuartzConfig : IQuartzConfig
    {
        private const string QuartzConnectionStringName = "Quartz";

        public string ConnectionString  => Environment.GetEnvironmentVariable(QuartzConnectionStringName)
                                           ?? "Server=localhost,1400; Database = Quartz; User = sa; Password = P@ssw0rd1; MultipleActiveResultSets = True";

        public string StorageType => "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";

        public NameValueCollection Settings
            =>
                new NameValueCollection
                {
                    ["quartz.jobStore.type"] = StorageType,
                    ["quartz.jobStore.clustered"] = "false",
                    ["quartz.jobStore.dataSource"] = "default",
                    ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                    ["quartz.jobStore.lockHandler.type"] =
                    "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                    ["quartz.dataSource.default.connectionString"] = ConnectionString,
                    ["quartz.dataSource.default.provider"] = "SqlServer",
                    ["quartz.scheduler.instanceId"] = "ScheduledEvents",
                    ["quartz.serializer.type"] = "json"
                };

        public string Name { get; }
        public IRetrySettings RetryOptions { get; set; } = new InMemoryRetrySettings();
    }
}