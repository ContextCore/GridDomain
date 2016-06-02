using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using GridDomain.Scheduling.Quartz.Logging;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace GridDomain.Scheduling.Quartz
{
    public class ContainerHolder
    {
        public static void Set(UnityContainer container)
        {
            Current = container;
        }
        public static UnityContainer Current { get; private set; }
    }

    public class JobFactory : IJobFactory
    {
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)ContainerHolder.Current.Resolve(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }

    public class SchedulerFactory : ISchedulerFactory
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

        public IScheduler GetScheduler()
        {
            return GetScheduler(null);
        }

        public IScheduler GetScheduler(string schedName)
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

            if (schedName != null)
            {
                properties["quartz.scheduler.instanceName"] = schedName;
            }

            var stdSchedulerFactory = new StdSchedulerFactory(properties);
            
            stdSchedulerFactory.Initialize();
            var scheduler = stdSchedulerFactory.GetScheduler();
            scheduler.JobFactory = new JobFactory();
            scheduler.ListenerManager.AddSchedulerListener(_loggingSchedulerListener);
            scheduler.ListenerManager.AddJobListener(_loggingJobListener);
            return scheduler;
        }

        public ICollection<IScheduler> AllSchedulers => new List<IScheduler>();
    }
}