using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AkkaCommandLocalWaiter : LocalMessagesWaiter<IExpectedCommandExecutor>
    {
        public AkkaCommandLocalWaiter(ICommandExecutor executor,ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout, bool failOnAnyFault) 
            : base(system, subscriber, defaultTimeout)
        {
            ExpectBuilder = new CommandExpectBuilder(executor, this, failOnAnyFault);
        }

        public ExpectBuilder<IExpectedCommandExecutor> ExpectBuilder { get; }
        public override IExpectBuilder<IExpectedCommandExecutor> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return ExpectBuilder.And(filter);
        }

        public override IExpectBuilder<IExpectedCommandExecutor> Expect(Type type, Func<object, bool> filter = null)
        {
            return ExpectBuilder.And(type, filter ?? (o => true));
        }

    }
}