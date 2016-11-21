using System;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Framework
{
    public class DebugLocalWaiter : LocalMessagesWaiter<AnyMessagePublisher>
    {
        public DebugLocalWaiter(IPublisher publisher, ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
            : base(system, subscriber, defaultTimeout)
        {
            ExpectBuilder = new AnyMessageExpectBuilder(publisher, this, defaultTimeout);
        }
        public override ExpectBuilder<AnyMessagePublisher> ExpectBuilder { get; }
    }
}