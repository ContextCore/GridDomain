using System;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public class NotifyOnPersistenceEvents : IHaveId
    {
        public static readonly NotifyOnPersistenceEvents Instance = new NotifyOnPersistenceEvents(null, null);

        public NotifyOnPersistenceEvents(IActorRef waiter, string id)
        {
            Waiter = waiter;
            Id = id;
        }

        public IActorRef Waiter { get; }
        public string Id { get; }
    }
}