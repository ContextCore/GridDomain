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
            Predicate<ExpectedMessage> isCommandFault = e => typeof(IMessageFault).IsAssignableFrom(e.MessageType);
            if (expectedMessage.Any(e => isCommandFault(e))) return expectedMessage;

            //TODO: replace it all with inheritance lookup in waiter

            //only available base class for fault message
            var commandFaultGenericType = typeof(MessageFault<>).MakeGenericType(command.GetType());

            var genericfaultExpect = new ExpectedMessage(commandFaultGenericType, 1, nameof(IMessageFault.Id), command.Id);

            expectedMessage = expectedMessage.Concat(new[] {genericfaultExpect }).ToArray();
            return expectedMessage;
        }
    }
}