using System;
using Quartz;
using Quartz.Listener;

namespace GridDomain.Scheduling.Quartz
{
    public class RetryJobListener : JobListenerSupport
    {
        private readonly IRetryStrategy _retryStrategy;
        public RetryJobListener(IRetryStrategy retryStrategy)
        {
            this._retryStrategy = retryStrategy;
        }
        public override string Name => "Retry";

        public override void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (JobFailed(jobException) && this._retryStrategy.ShouldRetry(context))
            {
                ITrigger trigger = this._retryStrategy.GetTrigger(context);
                bool unscheduled = context.Scheduler.UnscheduleJob(context.Trigger.Key);
                DateTimeOffset nextRunAt = context.Scheduler.ScheduleJob(context.JobDetail, trigger);
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