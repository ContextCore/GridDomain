using System;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class NotifyOnPersistenceEvents : IHaveId
    {
        public static readonly NotifyOnPersistenceEvents Instance = new NotifyOnPersistenceEvents(null, Guid.Empty);

        public NotifyOnPersistenceEvents(IActorRef waiter, Guid id)
        {
            Waiter = waiter;
            Id = id;
        }

        public IActorRef Waiter { get; }
        public Guid Id { get; }
    }
}