using System;

namespace GridDomain.Node.Actors.Sagas.Messages
{
    public class CommandCompleted
    {
        public Guid CommandId { get; }

        public CommandCompleted(Guid commandId)
        {
            CommandId = commandId;
        }

        public static CommandCompleted Instance { get; } = new CommandCompleted(Guid.Empty);
    }
}