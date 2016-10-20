using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AkkaCommandLocalWaiter : LocalMessagesWaiter<IExpectedCommandExecutor>
    {
        private readonly ICommandExecutor _executor;

        public AkkaCommandLocalWaiter(ICommandExecutor executor,ActorSystem system, IActorSubscriber subscriber) : base(system, subscriber)
        {
            _executor = executor;
            ExpectBuilder = new CommandExpectBuilder(_executor, this, TimeSpan.FromSeconds(10));
        }

        internal override ExpectBuilder<IExpectedCommandExecutor> ExpectBuilder { get; } 
    }
}