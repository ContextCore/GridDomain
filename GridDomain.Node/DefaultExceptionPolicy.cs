using System;
using GridDomain.Common;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Scheduling.Quartz.Retry;

namespace GridDomain.Node
{
    public class DefaultExceptionPolicy : IExceptionPolicy
    {
        public bool ShouldContinue(Exception ex)
        {
            if (ex.UnwrapSingle() is NullReferenceException)
                return false;

            if (ex.UnwrapSingle() is ScheduledEventNotFoundException)
                return false;

            return true;
        }
    }
}