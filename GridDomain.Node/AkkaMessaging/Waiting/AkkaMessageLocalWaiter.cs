using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Timers;
using Akka.Actor;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AkkaMessageLocalWaiter : LocalMessagesWaiter<Task<IWaitResults>>
    {
        public AkkaMessageLocalWaiter(ActorSystem system, IActorSubscriber subscriber) : base(system, subscriber)
        {
            ExpectBuilder = new MessageExpectBuilder(this, TimeSpan.FromSeconds(10));
        }

        internal override ExpectBuilder<Task<IWaitResults>> ExpectBuilder { get; }
    }
}