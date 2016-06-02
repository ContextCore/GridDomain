using System;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailingTestRequestHandler : TestRequestHandler<FailTaskRequest>
    {
        public FailingTestRequestHandler()
            : base(request => { throw new InvalidOperationException(); })
        {
        }
    }
}