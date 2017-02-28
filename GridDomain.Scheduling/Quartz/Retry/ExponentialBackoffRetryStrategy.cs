using System;
using GridDomain.Common;
using Quartz;
using Serilog;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class ExponentialBackoffRetryStrategy : IRetryStrategy
    {
        private const string Retries = "Retries";
        private readonly ILogger _log;
        private readonly IRetrySettings _settings;

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
                    context.JobDetail.Key.Name,
                    e.InnerException.GetType());
                return false;
            }

            var retries = GetAlreadyPerformedRetries(context);
            var shouldRetry = retries < _settings.MaxRetries;

            if (shouldRetry)
                _log.Debug("Job {Key} will be retried, {retries} / {maxRetries}",
                    context.JobDetail.Key.Name,
                    retries,
                    _settings.MaxRetries);
            else
                _log.Debug("Job {Key} will not be retried, as retries limit was reached: {maxRetries}",
                    context.JobDetail.Key.Name,
                    _settings.MaxRetries);

            return shouldRetry;
        }

        public ITrigger GetTrigger(IJobExecutionContext context)
        {
            var retries = GetAlreadyPerformedRetries(context);
            var factor = (long) Math.Pow(2, retries);
            var backoff = new TimeSpan(_settings.BackoffBaseInterval.Ticks*factor);

            var trigger =
                TriggerBuilder.Create()
                              .StartAt(BusinessDateTime.UtcNow + backoff)
                              .WithSimpleSchedule(x => x.WithRepeatCount(0))
                              .WithIdentity(context.Trigger.Key)
                              .ForJob(context.JobDetail)
                              .Build();

            context.JobDetail.JobDataMap[Retries] = ++retries;
            return trigger;
        }

        private static int GetAlreadyPerformedRetries(IJobExecutionContext context)
        {
            var retries = 0;
            object o;
            if (context.JobDetail.JobDataMap.TryGetValue(Retries, out o) && o is int) { retries = (int) o; }
            return retries;
        }
    }
}