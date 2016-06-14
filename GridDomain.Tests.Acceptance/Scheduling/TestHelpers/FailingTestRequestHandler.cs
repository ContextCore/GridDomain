using System;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailingTestRequestHandler : TestRequestHandler<FailTaskCommand>
    {
        public FailingTestRequestHandler()
            : base(request => { throw new InvalidOperationException(); })
        {
        }
    }
}