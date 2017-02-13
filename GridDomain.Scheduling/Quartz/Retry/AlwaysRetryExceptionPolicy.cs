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

    public class NeverRetryExceptionPolicy : IExceptionPolicy
    {
        public bool ShouldContinue(Exception ex)
        {
            return false;
        }
    }
}