using System.Collections.Generic;
using System.Collections.Specialized;
using GridDomain.Scheduling.Quartz.Logging;
using Quartz;
using Quartz.Impl;

namespace GridDomain.Scheduling.Quartz
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private readonly IQuartzConfig _config;
        private readonly ILoggingSchedulerListener _loggingSchedulerListener;
        private readonly ILoggingJobListener _loggingJobListener;
        private static readonly object _locker = new object();
        private IScheduler _current;
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

        public IScheduler GetScheduler()
        {
            return GetScheduler(null);
        }

        public IScheduler GetScheduler(string schedName)
        {
            lock (_locker)
            {
                if (_current == null || _current.IsShutdown)
                {
                    _current = Create(schedName);
                }
            }
            return _current;
        }

        public ICollection<IScheduler> AllSchedulers => new[] { _current };

        private IScheduler Create(string name)
        {
            var properties = new NameValueCollection
            {
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = "true",
                ["quartz.jobStore.clustered"] = "true",
                ["quartz.scheduler.instanceId"] = "AUTO",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz",
                ["quartz.dataSource.default.connectionString"] = _config.ConnectionString,
                ["quartz.dataSource.default.provider"] = "SqlServer-20"
            };

            if (name != null)
            {
                properties["quartz.scheduler.instanceName"] = name;
            }

            var stdSchedulerFactory = new StdSchedulerFactory(properties);

            stdSchedulerFactory.Initialize();
            var scheduler = stdSchedulerFactory.GetScheduler();
            scheduler.JobFactory = new JobFactory();
            scheduler.ListenerManager.AddSchedulerListener(_loggingSchedulerListener);
            scheduler.ListenerManager.AddJobListener(_loggingJobListener);

            scheduler.Start();
            return scheduler;
        }
    }
}