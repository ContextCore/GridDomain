using System;
using System.Threading.Tasks;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class TestRequestHandler<TRequest> : ScheduledTaskHandler<TRequest> where TRequest : ScheduledRequest
    {
        private readonly Func<TRequest, Task> _handler;

        public TestRequestHandler(Func<TRequest, Task> handler)
        {
            _handler = handler;
        }

        protected override async Task Handle(TRequest request)
        {
            await _handler(request);
        }
    }
}