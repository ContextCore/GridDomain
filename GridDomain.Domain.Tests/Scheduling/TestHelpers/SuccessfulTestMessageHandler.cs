namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class SuccessfulTestMessageHandler : TestRequestHandler<TestMessage>
    {
        public SuccessfulTestMessageHandler() 
                : base(request => ResultHolder.Add(request.TaskId, request.TaskId))
        {
        }
    }
}