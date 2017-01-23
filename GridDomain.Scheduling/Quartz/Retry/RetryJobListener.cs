using System;
using GridDomain.Logging;
using Quartz;
using Quartz.Listener;
using Serilog;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class RetryJobListener : JobListenerSupport
    {
        private readonly IRetryStrategy _retryStrategy;
        public RetryJobListener(IRetryStrategy retryStrategy)
        {
            this._retryStrategy = retryStrategy;
        }
        public override string Name => "Retry";
        private readonly ILogger _logger = Serilog.Log.Logger.ForContext<RetryJobListener>();

        public override void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (JobFailed(jobException) && this._retryStrategy.ShouldRetry(context, jobException))
            {
                ITrigger trigger = this._retryStrategy.GetTrigger(context);
                bool unscheduled = context.Scheduler.UnscheduleJob(context.Trigger.Key);
                DateTimeOffset nextRunAt = context.Scheduler.ScheduleJob(context.JobDetail, trigger);
                _logger.Warning("Restarting job {key}, unsheduling was {unscheduled}, next start time {nextRunAt}", 
                                        context.JobDetail.Key.Name,
                                        unscheduled,
                                        nextRunAt);
            }
        }
        public override void JobToBeExecuted(IJobExecutionContext context)
        {
        }
        private static bool JobFailed(JobExecutionException jobException)
        {
            return jobException != null;
        }
    }
}