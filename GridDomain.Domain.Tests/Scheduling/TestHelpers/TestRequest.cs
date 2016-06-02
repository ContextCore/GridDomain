using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class TestRequest : ScheduledRequest
    {
        public TestRequest(string taskId = null) : base(taskId)
        {
        }
    }
}