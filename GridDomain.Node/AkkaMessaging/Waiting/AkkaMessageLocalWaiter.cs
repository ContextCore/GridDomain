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

        public ExpectBuilder<Task<IWaitResults>> ExpectBuilder { get; }

        public override IExpectBuilder<Task<IWaitResults>> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return ExpectBuilder.And(filter);
        }
        public override IExpectBuilder<Task<IWaitResults>> Expect(Type type, Func<object, bool> filter = null)
        {
            return ExpectBuilder.And(type, filter ?? (o => true));
        }
    }
}