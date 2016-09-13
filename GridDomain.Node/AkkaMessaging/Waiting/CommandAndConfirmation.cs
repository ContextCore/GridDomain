using System;
using System.Linq;
using System.Threading;
using GridDomain.CQRS;
using MemBus.Support;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandPlan
    {
        public ExpectedMessage[] ExpectedMessages { get; }
        public ICommand Command { get; }

        public TimeSpan Timeout { get;}
        public CommandPlan(ICommand command, params ExpectedMessage[] expectedMessage)
            :this(command, GridDomainNode.DefaultCommandTimeout, expectedMessage)
        {
        }

        public CommandPlan(ICommand command, TimeSpan timeout, params ExpectedMessage[] expectedMessage)
        {
            expectedMessage = AddCommandFaultIfMissing(command, expectedMessage);
            ExpectedMessages = expectedMessage;
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
            var existingFault = expectedMessage.OfType<ExpectedFault>().FirstOrDefault();
            if (existingFault == null) return expectedMessage;

            var commandType = command.GetType();
            if(existingFault.MessageType == commandType) return expectedMessage;

            var expectedFault = ExpectedFault.New(commandType, nameof(ICommand.Id), command.Id);
            return expectedMessage.Concat(new[] { expectedFault }).ToArray(); ;
        }
    }
}