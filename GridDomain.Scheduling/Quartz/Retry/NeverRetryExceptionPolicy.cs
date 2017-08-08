using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class NeverRetryExceptionPolicy : IExceptionPolicy
    {
        public bool ShouldContinue(Exception ex)
        {
            return false;
        }
    }
}