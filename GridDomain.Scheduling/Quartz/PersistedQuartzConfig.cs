using System.Collections.Specialized;
using System.Configuration;

namespace GridDomain.Scheduling.Quartz
{
    public class PersistedQuartzConfig : IQuartzConfig
    {
        private const string QuartzConnectionStringName = "Quartz";

        public string ConnectionString => ConfigurationManager.ConnectionStrings[QuartzConnectionStringName]?.ConnectionString 
            ??
            "Server=tcp:tulyov9rv7.database.windows.net,1433;Database=MembershipScheduler_Copy;User ID=SoloProdSQL-User@tulyov9rv7;Password=Vecrfn0Rhfcysq;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;MultipleActiveResultSets=True;Application Name=Quartz;";
        public string StorageType => "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";

        public NameValueCollection Settings => new NameValueCollection
        {
            ["quartz.jobStore.type"] = StorageType,
            ["quartz.jobStore.clustered"] = "true",
            ["quartz.jobStore.dataSource"] = "default",
            ["quartz.jobStore.tablePrefix"] = "QRTZ_",
            ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
            ["quartz.dataSource.default.connectionString"] = ConnectionString,
            ["quartz.dataSource.default.provider"] = "SqlServer-20",
            ["quartz.scheduler.instanceId"] = "AUTO"
        };
    }
}