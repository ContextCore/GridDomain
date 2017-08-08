using System;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class DefaultExceptionPolicy : IExceptionPolicy
    {
        public bool ShouldContinue(Exception ex)
        {
            var businessException = ex.UnwrapSingle();

            if (businessException is NullReferenceException)
                return false;

            if (businessException is ScheduledEventNotFoundException)
                return false;

            if (businessException is CommandExecutionFailedException 
                && businessException.InnerException is ScheduledEventNotFoundException)
                return false;

            return true;
        }
    }
}