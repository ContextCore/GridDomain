using System;
using GridDomain.Logging;


namespace GridDomain.Scheduling.Quartz.Logging
{
    public class QuartzLogger : IQuartzLogger
    {
        private readonly ISoloLogger _coreLogger;

        public QuartzLogger()
        {
            _coreLogger = LogManager.GetLogger();
        }

        public void LogSuccess(string jobName)
        {
            _coreLogger.Trace("Job {JobName} sucessfully finished", jobName);
        }

        public void LogFailure(string jobName, Exception e)
        {
            _coreLogger.Error(e, "Job {JobName} got an error", jobName);
        }
    }
}