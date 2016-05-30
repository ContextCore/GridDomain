using System.Threading.Tasks;
using GridDomain.Scheduling.Akka;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class SuccessfulTestRequestHandler : ScheduledTaskHandlerActorBase<TestRequest>
    {
        protected override Task Handle(TestRequest request) => Task.FromResult(true);
    }
}