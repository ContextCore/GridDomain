using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;

using GridDomain.Node.Transports;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class LocalExplicitMessagesWaiter : LocalMessagesWaiter<Task<IWaitResult>>
    {
        public LocalExplicitMessagesWaiter(ActorSystem system,
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout) : this(system, subscriber, defaultTimeout, new ConditionBuilder<Task<IWaitResult>>()) {}

        public LocalExplicitMessagesWaiter(ActorSystem system, 
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout, 
                                           ConditionBuilder<Task<IWaitResult>> conditionBuilder) : base(system, subscriber, defaultTimeout, conditionBuilder)
        {
            conditionBuilder.CreateResultFunc = Start;
        }
    }
}