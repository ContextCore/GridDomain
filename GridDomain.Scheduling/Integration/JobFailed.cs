using System;

namespace GridDomain.Scheduling.Integration
{
    public class JobFailed : JobCompleted
    {
        public Exception Error { get; }

        public JobFailed(string name, string group, Exception error) : base(name, group)
        {
            Error = error;
        }
    }
}