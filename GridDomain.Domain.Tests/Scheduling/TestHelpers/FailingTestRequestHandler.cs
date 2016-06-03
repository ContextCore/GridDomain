using System;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailingTestRequestHandler : TestRequestHandler<FailTaskMessage>
    {
        public FailingTestRequestHandler()
            : base(request => { throw new InvalidOperationException(); })
        {
        }
    }
}