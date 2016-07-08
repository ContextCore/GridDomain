using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandAndConfirmation
    {
        public ExpectedMessage[] ExpectedMessages { get; }
        public ICommand Command { get; }

        public CommandAndConfirmation(ICommand command, params ExpectedMessage[] expectedMessage)
        {
            ExpectedMessages = expectedMessage;
            Command = command;
        }
    }
}