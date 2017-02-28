using System;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class NotifyOnPersistenceEvents : IPersistenceEntityActorMaintainanceMessage
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