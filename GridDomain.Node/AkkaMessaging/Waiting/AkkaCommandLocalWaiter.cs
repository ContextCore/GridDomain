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
            ExpectBuilder = new CommandExpectBuilder(executor, this, defaultTimeout, failOnAnyFault);
        }

        internal override ExpectBuilder<IExpectedCommandExecutor> ExpectBuilder { get; } 
    }
}