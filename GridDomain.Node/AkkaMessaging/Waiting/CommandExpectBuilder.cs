using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandExpectBuilder : ExpectBuilder<IExpectedCommandExecutor>
    {
        private readonly ICommandExecutor _executor;

        public CommandExpectBuilder(ICommandExecutor executor, LocalMessagesWaiter<IExpectedCommandExecutor> waiter, TimeSpan defaultTimeout) : base(waiter, defaultTimeout)
        {
            _executor = executor;
        }

        public override IExpectedCommandExecutor Create(TimeSpan? timeout=null)
        {
            return new ExpectedCommandExecutor(_executor,_waiter, timeout ?? DefaultTimeout);
        }
    }
}