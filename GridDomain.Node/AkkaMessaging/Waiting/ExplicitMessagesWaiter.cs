using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExplicitMessagesWaiter : MessagesWaiter
    {
        public ExplicitMessagesWaiter(ActorSystem system,
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout) : this(system, subscriber, defaultTimeout, new ConditionFactory<Task<IWaitResult>>()) {}

        public ExplicitMessagesWaiter(ActorSystem system, 
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout, 
                                           ConditionFactory<Task<IWaitResult>> conditionFactory) : base(system, subscriber, defaultTimeout, conditionFactory)
        {
            conditionFactory.CreateResultFunc = Start;
        }
    }
}