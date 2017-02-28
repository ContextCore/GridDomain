using Quartz;
using Serilog;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public class LoggingJobListener : IJobListener
    {
        private readonly ILogger _log;

        public LoggingJobListener(ILogger log)
        {
            _log = log.ForContext<LoggingJobListener>();
        }

        public string Name => GetType()
            .Name;

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            _log.Debug("Job {JobKey} is about to be executed", context.JobDetail.Key);
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            _log.Debug("Job {JobKey} execution vetoed", context.JobDetail.Key);
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            _log.Debug("Job {JobKey} was executed", context.JobDetail.Key);
        }
    }
}