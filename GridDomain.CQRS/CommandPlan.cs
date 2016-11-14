using System;
using System.Linq;

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

    public class CommandPlan
    {
        public ExpectedMessage[] ExpectedMessages { get; }
        public ICommand Command { get; }

        public TimeSpan Timeout { get; }
        public CommandPlan(ICommand command, params ExpectedMessage[] expectedMessage)
                     : this(command, TimeSpan.FromSeconds(10), expectedMessage)
        {
        }

        public CommandPlan(ICommand command, TimeSpan timeout, params ExpectedMessage[] expectedMessage)
        {
            ExpectedMessages = AddCommandFaultIfMissing(command, expectedMessage);
            Command = command;
            Timeout = timeout;
        }
        public static CommandPlan<T> New<T>(ICommand command, TimeSpan timeout, ExpectedMessage<T> expectedMessage)
        {
            return new CommandPlan<T>(command, timeout, expectedMessage);
        }

        public static CommandPlan<T> New<T>(ICommand command, ExpectedMessage<T> expectedMessage)
        {
            return new CommandPlan<T>(command, expectedMessage);
        }

        private static ExpectedMessage[] AddCommandFaultIfMissing(ICommand command, ExpectedMessage[] expectedMessage)
        {
            var commandType = command.GetType();
            if (expectedMessage.OfType<ExpectedFault>().Any(f => f.ProcessMessageType == commandType))
                return expectedMessage;

            var expectedFault = ExpectedFault.New(commandType, nameof(ICommand.Id), command.Id);
            return expectedMessage.Concat(new[] { expectedFault }).ToArray(); ;
        }
    }
}