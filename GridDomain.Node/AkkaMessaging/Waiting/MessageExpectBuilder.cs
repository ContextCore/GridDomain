using System;
using System.Threading.Tasks;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class MessageExpectBuilder : ExpectBuilder<Task<IWaitResults>>
    {
        public MessageExpectBuilder(LocalMessagesWaiter<Task<IWaitResults>> waiter, TimeSpan defaultTimeout) : base(waiter, defaultTimeout)
        {
        }

        public override Task<IWaitResults> Create(TimeSpan timeout)
        {
            return _waiter.Start(timeout);
        }
    }
}