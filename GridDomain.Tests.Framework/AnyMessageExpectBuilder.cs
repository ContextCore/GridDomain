using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Framework
{
    public class AnyMessageExpectBuilder : ExpectBuilder<AnyMessagePublisher>
    {
        private readonly CommandPipeBuilder _commandPipe;

        public AnyMessageExpectBuilder(CommandPipeBuilder commandPipe,
                                       LocalMessagesWaiter<AnyMessagePublisher> waiter,
                                       TimeSpan defaultTimeout) : base(waiter)
        {
            _commandPipe = commandPipe;
        }

        public override AnyMessagePublisher Create(TimeSpan? timeout)
        {
            return new AnyMessagePublisher(_commandPipe, Waiter);
        }
    }
}