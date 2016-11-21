using System;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Framework
{
    public class AnyMessageExpectBuilder : ExpectBuilder<AnyMessagePublisher>
    {
        private readonly IPublisher _publisher;

        public AnyMessageExpectBuilder(IPublisher publisher, LocalMessagesWaiter<AnyMessagePublisher> waiter, TimeSpan defaultTimeout) : base(waiter)
        {
            _publisher = publisher;
        }

        public override AnyMessagePublisher Create(TimeSpan? timeout)
        {
            return new AnyMessagePublisher(_publisher,Waiter);
        }
    }
}