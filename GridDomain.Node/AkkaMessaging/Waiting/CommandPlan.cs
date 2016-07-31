using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandPlan<T> : CommandPlan
    {
        public CommandPlan(ICommand command, params ExpectedMessage[] expectedMessage) : base(command, expectedMessage)
        {
        }

        public CommandPlan(ICommand command, TimeSpan timeout, params ExpectedMessage[] expectedMessage) : base(command, timeout, expectedMessage)
        {
        }
    }
}