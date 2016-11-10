using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class MessageExpectBuilder : ExpectBuilder<Task<IWaitResults>>
    {
        public MessageExpectBuilder(LocalMessagesWaiter<Task<IWaitResults>> waiter, TimeSpan defaultTimeout) : base(waiter, defaultTimeout)
        {
        }

        public override Task<IWaitResults> Create(TimeSpan? timeout = null)
        {
            return _waiter.Start(timeout ?? DefaultTimeout);
        }
    }
}