using NLog;
using Quartz;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public class LoggingJobListener : ILoggingJobListener
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public string Name => GetType().Name;

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            _log.Debug($"Job {context.JobDetail.Key} is about to be executed");
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            _log.Debug($"Job {context.JobDetail.Key} execution vetoed");
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            _log.Debug($"Job {context.JobDetail.Key} was executed");
        }
    }
}