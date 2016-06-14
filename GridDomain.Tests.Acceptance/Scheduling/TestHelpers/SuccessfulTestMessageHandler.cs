namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class SuccessfulTestMessageHandler : TestRequestHandler<TestCommand>
    {
        public SuccessfulTestMessageHandler(ResultHolder resultHolder) 
                : base(request => resultHolder.Add(request.TaskId, request.TaskId))
        {
        }
    }
}