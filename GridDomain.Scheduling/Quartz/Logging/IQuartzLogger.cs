using System;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public interface IQuartzLogger
    {
        void LogSuccess(string jobName);
        void LogFailure(string jobName, Exception e);
        void LogWarn(string jobName, string message);
        void LogInfo(string jobName, string message);
    }
}