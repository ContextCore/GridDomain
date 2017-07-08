using System;

namespace GridDomain.Scheduling.Quartz
{
    public class JobFailed : JobCompleted
    {
        public JobFailed(string name, string group, Exception error, object processingMessage) : base(name, group)
        {
            Error = error;
            ProcessingMessage = processingMessage;
        }

        public Exception Error { get; }
        public object ProcessingMessage { get; }
    }
}