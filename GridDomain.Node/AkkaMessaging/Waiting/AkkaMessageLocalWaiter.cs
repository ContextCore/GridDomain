using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Timers;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AkkaMessageLocalWaiter : LocalMessagesWaiter<Task<IWaitResults>>
    {
        public AkkaMessageLocalWaiter(ActorSystem system, IActorSubscriber subscriber, TimeSpan defaultTimeout) : base(system, subscriber, defaultTimeout)
        {
            ExpectBuilder = new MessageExpectBuilder(this, defaultTimeout);
        }

        public override ExpectBuilder<Task<IWaitResults>> ExpectBuilder { get; }
    }
}