namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class SuccessfulTestRequestHandler : TestRequestHandler<TestRequest>
    {
        public SuccessfulTestRequestHandler() : base(async request => ResultHolder.Add(request.TaskId, request.TaskId))
        {
        }
    }
}