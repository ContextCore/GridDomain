using Quartz;

namespace GridDomain.Scheduling.Quartz
{
    public interface IRetryStrategy
    {
        bool ShouldRetry(IJobExecutionContext context);
        ITrigger GetTrigger(IJobExecutionContext context);
    }
}