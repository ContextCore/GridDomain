namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class SuccessfulTestMessageHandler : TestRequestHandler<TestMessage>
    {
        public SuccessfulTestMessageHandler(ResultHolder resultHolder) 
                : base(request => resultHolder.Add(request.TaskId, request.TaskId))
        {
        }
    }
}