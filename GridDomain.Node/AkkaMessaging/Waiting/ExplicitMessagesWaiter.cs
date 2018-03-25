using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExplicitMessagesWaiter : MessagesWaiter<Task<IWaitResult>>
    {
        public ExplicitMessagesWaiter(ActorSystem system,
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout) : this(system, subscriber, defaultTimeout, new ConditionBuilder<Task<IWaitResult>>()) {}

        public ExplicitMessagesWaiter(ActorSystem system, 
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout, 
                                           ConditionBuilder<Task<IWaitResult>> conditionBuilder) : base(system, subscriber, defaultTimeout, conditionBuilder)
        {
            conditionBuilder.CreateResultFunc = Start;
        }
    }
}