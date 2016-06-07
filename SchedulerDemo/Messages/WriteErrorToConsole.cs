using System;

namespace SchedulerDemo.Messages
{
    public class WriteErrorToConsole
    {
        public Exception Exception { get;}

        public WriteErrorToConsole(Exception exception)
        {
            Exception = exception;
        }
    }
}