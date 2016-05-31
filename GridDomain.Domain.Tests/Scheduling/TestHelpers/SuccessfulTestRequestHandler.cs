using System.Threading.Tasks;
using GridDomain.Scheduling.Akka;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class SuccessfulTestRequestHandler : ScheduledTaskHandler<TestRequest>
    {
        protected override async Task Handle(TestRequest request)
        {
            ResultHolder.Add(request.TaskId, request.TaskId);
        }
    }
}