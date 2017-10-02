using System.Threading;
using System.Threading.Tasks;
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

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            _log.Debug("Job {JobKey} is about to be executed", context.JobDetail.Key);
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            _log.Debug("Job {JobKey} execution vetoed", context.JobDetail.Key);
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = new CancellationToken())
        {
            _log.Debug("Job {JobKey} was executed", context.JobDetail.Key);
            return Task.CompletedTask;
        }

        public string Name => GetType().Name;
    }
}
