using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public interface IExceptionPolicy
    {
        bool ShouldContinue(Exception ex);
    }
}