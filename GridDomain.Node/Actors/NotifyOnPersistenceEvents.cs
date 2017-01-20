using System;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class NotifyOnPersistenceEvents : IPersistenceEntityActorMaintainanceMessage
    {
        public Guid Id { get; }
        public NotifyOnPersistenceEvents(IActorRef waiter, Guid? id = null)
        {
            Waiter = waiter;
            Id = id ?? Guid.Empty;
        }

        public static readonly NotifyOnPersistenceEvents Instance = new NotifyOnPersistenceEvents(null);

        public IActorRef Waiter { get; }
    }
}