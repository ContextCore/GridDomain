using System;

namespace SchedulerDemo.Messages
{
    public class WriteError
    {
        public Exception Exception { get;}

        public WriteError(Exception exception)
        {
            Exception = exception;
        }
    }
}