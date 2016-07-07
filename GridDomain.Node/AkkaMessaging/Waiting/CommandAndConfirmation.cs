using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandAndConfirmation
    {
        public TimeSpan Timeout { get; }
        public ExpectedMessage[] ExpectedMessages { get; }
        public ICommand Command { get; }

        public CommandAndConfirmation(ICommand command, TimeSpan timeout, params ExpectedMessage[] expectedMessage)
        {
            Timeout = timeout;
            ExpectedMessages = expectedMessage;
            Command = command;
        }
    }
}