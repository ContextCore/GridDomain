using System;
using System.Threading.Tasks;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class TestRequestHandler<TRequest> : ScheduledTaskHandler<TRequest> where TRequest : ScheduledRequest
    {
        private readonly Action<TRequest> _handler;

        public TestRequestHandler(Action<TRequest> handler)
        {
            _handler = handler;
        }

        protected override void Handle(TRequest request)
        {
            _handler(request);
        }
    }
}