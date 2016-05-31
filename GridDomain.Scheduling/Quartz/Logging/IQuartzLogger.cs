using System;

namespace GridDomain.Scheduling.Quartz.Logging
{
    public interface IQuartzLogger
    {
        void LogSuccess(string jobName);
        void LogFailure(string jobName, Exception e);
    }
}