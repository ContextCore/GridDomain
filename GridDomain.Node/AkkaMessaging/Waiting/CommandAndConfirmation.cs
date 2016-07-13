using System;
using System.Linq;
using GridDomain.CQRS;
using MemBus.Support;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandAndConfirmation
    {
        public ExpectedMessage[] ExpectedMessages { get; }
        public ICommand Command { get; }

        public CommandAndConfirmation(ICommand command, params ExpectedMessage[] expectedMessage)
        {
            expectedMessage = AddCommandFaultIfMissing(command, expectedMessage);
            ExpectedMessages = expectedMessage;
            Command = command;
        }

        private static ExpectedMessage[] AddCommandFaultIfMissing(ICommand command, ExpectedMessage[] expectedMessage)
        {
            Predicate<ExpectedMessage> isCommandFault = e => typeof(ICommandFault).IsAssignableFrom(e.MessageType);
            if (expectedMessage.Any(e => isCommandFault(e))) return expectedMessage;

            //TODO: replace it all with inheritance lookup in waiter

            //only available base class for fault message
            var commandFaultGenericType = typeof(CommandFault<>).MakeGenericType(command.GetType());

            var genericfaultExpect = new ExpectedMessage(commandFaultGenericType, 1, nameof(ICommandFault.Id), command.Id);

            expectedMessage = expectedMessage.Concat(new[] {genericfaultExpect }).ToArray();
            return expectedMessage;
        }
    }
}