using System.Collections.Specialized;
using GridDomain.Scheduling.Quartz.Logging;
using Quartz;
using Quartz.Impl;

namespace GridDomain.Scheduling.Quartz
{
    public class SchedulerFactory
    {
        private readonly IQuartzConfig _config;
        private readonly ILoggingSchedulerListener _loggingSchedulerListener;
        private readonly ILoggingJobListener _loggingJobListener;

        public SchedulerFactory(
            IQuartzConfig config, 
            ILoggingSchedulerListener loggingSchedulerListener,
            ILoggingJobListener loggingJobListener
            )
        {
            _config = config;
            _loggingSchedulerListener = loggingSchedulerListener;
            _loggingJobListener = loggingJobListener;
        }

        public IScheduler Create()
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "TestScheduler",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = "true",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.dataSource.default.connectionString"] = _config.ConnectionString,
                ["quartz.dataSource.default.provider"] = "SqlServer-20"
            };

            var stdSchedulerFactory = new StdSchedulerFactory(properties);
            stdSchedulerFactory.Initialize();
            var scheduler = stdSchedulerFactory.GetScheduler();
            scheduler.ListenerManager.AddSchedulerListener(_loggingSchedulerListener);
            scheduler.ListenerManager.AddJobListener(_loggingJobListener);
            return scheduler;
        }
    }
}