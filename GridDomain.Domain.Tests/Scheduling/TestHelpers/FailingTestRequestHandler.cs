using System;
using System.Threading.Tasks;
using GridDomain.Scheduling.Akka;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailingTestRequestHandler : ScheduledTaskHandler<TestRequest>
    {
        protected override async Task Handle(TestRequest request)
        {
            throw new InvalidOperationException("something went wrong");
        }
    }
}