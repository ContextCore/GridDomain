using System;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class NotifyOnPersistenceEvents : IHaveId
    {
        public static readonly NotifyOnPersistenceEvents Instance = new NotifyOnPersistenceEvents(null);

        public NotifyOnPersistenceEvents(IActorRef waiter, Guid? id = null)
        {
            Waiter = waiter;
            Id = id ?? Guid.Empty;
        }

        public IActorRef Waiter { get; }
        public Guid Id { get; }
    }
}