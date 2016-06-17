using System;
using NLog;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public class QuartzLogger : IQuartzLogger
    {
        private readonly Logger _coreLogger;

        public QuartzLogger()
        {
            _coreLogger = LogManager.GetCurrentClassLogger();
        }

        public void LogSuccess(string jobName)
        {
            _coreLogger.Info($"Job {jobName} sucessfully finished");
        }

        public void LogFailure(string jobName, Exception e)
        {
            _coreLogger.Error(e, $"Job {jobName} got an error");
        }
    }
}