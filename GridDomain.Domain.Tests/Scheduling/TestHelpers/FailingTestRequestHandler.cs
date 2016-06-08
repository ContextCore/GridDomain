using System;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailingTestRequestHandler : TestRequestHandler<FailTaskCommand>
    {
        public FailingTestRequestHandler()
            : base(request => { throw new InvalidOperationException(); })
        {
        }
    }
}