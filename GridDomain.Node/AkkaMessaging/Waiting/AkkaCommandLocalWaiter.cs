using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AkkaCommandLocalWaiter : LocalMessagesWaiter<IExpectedCommandExecutor>
    {
        private readonly ICommandExecutor _executor;
        private readonly bool _failOnAnyFault;

        public AkkaCommandLocalWaiter(ICommandExecutor executor,ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout, bool failOnAnyFault) : base(system, subscriber, defaultTimeout)
        {
            _failOnAnyFault = failOnAnyFault;
            _executor = executor;
            ExpectBuilder = new CommandExpectBuilder(_executor, this, defaultTimeout,failOnAnyFault);
        }

        internal override ExpectBuilder<IExpectedCommandExecutor> ExpectBuilder { get; } 
    }
}