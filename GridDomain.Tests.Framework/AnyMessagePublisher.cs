using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Framework
{
    public class AnyMessagePublisher 
    {
        private readonly IPublisher _publisher;
        private readonly LocalMessagesWaiter<AnyMessagePublisher> _waiter;

        public AnyMessagePublisher(IPublisher publisher, LocalMessagesWaiter<AnyMessagePublisher> waiter)
        {
            _waiter = waiter;
            _publisher = publisher;

        }

        public async Task<IWaitResults> Publish(params object[] messages)
        {
            var task =_waiter.Start();
            _publisher.Publish(messages);
            return await task;

        }
    }
}