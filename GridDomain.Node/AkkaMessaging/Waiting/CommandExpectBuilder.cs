using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandExpectBuilder : ExpectBuilder<IExpectedCommandExecutor>
    {
        private readonly ICommandExecutor _executor;
        private readonly bool _failOnAnyFault;

        public CommandExpectBuilder(ICommandExecutor executor, LocalMessagesWaiter<IExpectedCommandExecutor> waiter, TimeSpan defaultTimeout, bool failOnAnyFault) : base(waiter)
        {
            _failOnAnyFault = failOnAnyFault;
            _executor = executor;
        }

        public override IExpectedCommandExecutor Create(TimeSpan timeout)
        {
            return new ExpectedCommandExecutor(_executor,Waiter, _failOnAnyFault);
        }
    }
}