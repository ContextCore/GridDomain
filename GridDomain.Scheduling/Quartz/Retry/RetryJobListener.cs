using Quartz;
using Quartz.Listener;
using Serilog;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class RetryJobListener : JobListenerSupport
    {
        private readonly ILogger _logger;
        private readonly IRetryStrategy _retryStrategy;

        public RetryJobListener(IRetryStrategy retryStrategy, ILogger log)
        {
            _retryStrategy = retryStrategy;
            _logger = log.ForContext<RetryJobListener>();
        }

        public override string Name => "Retry";

        public override void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            _logger.Debug("Deciding if job {job} should be retried", context.JobDetail.Key);
            if (!JobFailed(jobException) || !_retryStrategy.ShouldRetry(context, jobException))
                return;

            var trigger = _retryStrategy.GetTrigger(context);
            context.Scheduler.UnscheduleJob(context.Trigger.Key);
            context.Scheduler.ScheduleJob(context.JobDetail, trigger);
        }

        private static bool JobFailed(JobExecutionException jobException)
        {
            return jobException != null;
        }
    }
}