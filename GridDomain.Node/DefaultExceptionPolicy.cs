using System;
using GridDomain.Common;
using GridDomain.Node.Actors.Aggregates.Exceptions;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Scheduling.Quartz.Retry;

namespace GridDomain.Node
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