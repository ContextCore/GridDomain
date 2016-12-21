using System;

namespace GridDomain.Scheduling.Integration
{
    public class JobFailed : JobCompleted
    {
        public Exception Error { get; }
        public object ProcessingMessage { get;}

        public JobFailed(string name, string group, Exception error, object processingMessage) : base(name, group)
        {
            Error = error;
            ProcessingMessage = processingMessage;
        }
    }
}