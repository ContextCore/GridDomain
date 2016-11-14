using System;

namespace GridDomain.Node.Actors
{
    public class AggregatePersistedNotification
    {
        public AggregatePersistedNotification(Guid id, Guid commandId)
        {
            Id = id;
            CommandId = commandId;
        }

        public Guid CommandId { get; }
        public Guid Id { get; }
    }
}