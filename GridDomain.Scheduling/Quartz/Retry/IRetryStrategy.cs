using Quartz;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public interface IRetryStrategy
    {
        ITrigger GetTrigger(IJobExecutionContext context);
        bool ShouldRetry(IJobExecutionContext context, JobExecutionException jobException);
    }
}