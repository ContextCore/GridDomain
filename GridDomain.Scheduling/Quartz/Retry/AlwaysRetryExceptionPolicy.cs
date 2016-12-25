using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class AlwaysRetryExceptionPolicy : IExceptionPolicy
    {
        public bool ShouldContinue(Exception ex)
        {
            return true;
        }
    }
}