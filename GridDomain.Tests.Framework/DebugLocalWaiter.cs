using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Framework
{
    public class DebugLocalWaiter : LocalMessagesWaiter<AnyMessagePublisher>
    {
        public DebugLocalWaiter(CommandPipeBuilder commandPipe, ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout)
            : base(system, subscriber, defaultTimeout)
        {
            ExpectBuilder = new AnyMessageExpectBuilder(commandPipe, this, defaultTimeout);
        }
        public ExpectBuilder<AnyMessagePublisher> ExpectBuilder { get; }
        public override IExpectBuilder<AnyMessagePublisher> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return ExpectBuilder.And(filter);
        }

        public override IExpectBuilder<AnyMessagePublisher> Expect(Type type, Func<object, bool> filter = null)
        {
            return ExpectBuilder.And(type, filter ?? (o => true));
        }
    }
}