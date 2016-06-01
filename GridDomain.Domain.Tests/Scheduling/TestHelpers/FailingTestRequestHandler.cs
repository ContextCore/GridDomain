using System;
using System.Threading.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailingTestRequestHandler : TestRequestHandler<FailTaskRequest>
    {
        public FailingTestRequestHandler()
            : base(request => Task.FromException(new InvalidOperationException()))
        {
        }
    }
}