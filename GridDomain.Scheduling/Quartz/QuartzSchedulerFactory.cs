using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.Quartz.Retry;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Serilog;

namespace GridDomain.Scheduling.Quartz
{
    public class QuartzSchedulerFactory : StdSchedulerFactory

    {
        private readonly IJobFactory _jobFactory;
        private readonly ILogger _log;
        private readonly IRetryStrategy _retryStrategy;

        public QuartzSchedulerFactory(IJobFactory jobFactory, ILogger log, IRetryStrategy retryStrategy)
        {
            _log = log;
            _retryStrategy = retryStrategy;
            _jobFactory = jobFactory;
        }

        public override IScheduler GetScheduler()
        {
            var scheduler = base.GetScheduler();
            scheduler.JobFactory = _jobFactory;
            scheduler.ListenerManager.AddSchedulerListener(new LoggingSchedulerListener(_log));
            scheduler.ListenerManager.AddJobListener(new LoggingJobListener(_log), GroupMatcher<JobKey>.AnyGroup());
            scheduler.ListenerManager.AddJobListener(new RetryJobListener(_retryStrategy, _log),
                GroupMatcher<JobKey>.AnyGroup());
            return scheduler;
        }
    }
}