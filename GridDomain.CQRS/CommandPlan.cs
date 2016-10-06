using System;

namespace GridDomain.CQRS
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