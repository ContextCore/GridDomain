using System;
using GridDomain.Logging;
using Quartz;
using Serilog;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class ExponentialBackoffRetryStrategy : IRetryStrategy
    {
        private const string Retries = "Retries";
        private readonly IRetrySettings _settings;
        private readonly ILogger _log;

        public ExponentialBackoffRetryStrategy(IRetrySettings settings, ILogger log)
        {
            _settings = settings;
            _log = log.ForContext<ExponentialBackoffRetryStrategy>();
        }
        public bool ShouldRetry(IJobExecutionContext context, JobExecutionException e)
        {
            if (e?.InnerException != null && !_settings.ErrorActions.ShouldContinue(e.InnerException))
            {
                _log.Debug("Job {Key} will not be retried due to special error encoured: {error}",
                    context.JobDetail.Key.Name, e.InnerException);
                return false;
            }

            int retries = GetAlreadyPerformedRetries(context);
            var shouldRetry = retries < this._settings.MaxRetries;

            if(shouldRetry)
                _log.Debug("Job {Key} will be retried, {retries} / {maxRetries}",context.JobDetail.Key.Name, retries,_settings.MaxRetries);
            else 
                _log.Debug("Job {Key} will not be retried, as retries limit was reached: {maxRetries}", context.JobDetail.Key.Name,
                    _settings.MaxRetries);

            return shouldRetry;
        }
        public ITrigger GetTrigger(IJobExecutionContext context)
        {
            int retries = GetAlreadyPerformedRetries(context);
            long factor = (long)Math.Pow(2, retries);
            TimeSpan backoff = new TimeSpan(this._settings.BackoffBaseInterval.Ticks * factor);

            ITrigger trigger = TriggerBuilder.Create()
                                             .StartAt(DateTimeOffset.UtcNow + backoff)
                                             .WithSimpleSchedule(x => x.WithRepeatCount(0))
                                             .WithIdentity(context.Trigger.Key)
                                             .ForJob(context.JobDetail)
                                             .Build();

            context.JobDetail.JobDataMap[Retries] = ++retries;
            return trigger;
        }
        private static int GetAlreadyPerformedRetries(IJobExecutionContext context)
        {
            int retries = 0;
            object o;
            if (context.JobDetail.JobDataMap.TryGetValue(Retries, out o) && o is int)
            {
                retries = (int)o;
            }
            return retries;
        }
    }
}