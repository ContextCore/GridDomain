using System.Threading;
using System.Threading.Tasks;
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

        public override string Name => "RetryJobListener";

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken token)
        {
            if (!_retryStrategy.ShouldRetry(context, jobException))
                return;

            _logger.Information("job {job} will be retried", context.JobDetail.Key);
            var trigger = _retryStrategy.GetTrigger(context);
            await context.Scheduler.UnscheduleJob(context.Trigger.Key, token);
            await context.Scheduler.ScheduleJob(context.JobDetail, trigger, token);
            return;
        }
    }
}