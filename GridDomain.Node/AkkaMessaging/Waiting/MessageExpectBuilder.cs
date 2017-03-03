using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class MessageExpectBuilder : ExpectBuilder<Task<IWaitResults>>
    {
        public MessageExpectBuilder(LocalMessagesWaiter<Task<IWaitResults>> waiter, TimeSpan defaultTimeout) : base(waiter) {}

        protected override Task<IWaitResults> Create(TimeSpan? timeout)
        {
            return Waiter.Start(timeout);
        }
    }
}