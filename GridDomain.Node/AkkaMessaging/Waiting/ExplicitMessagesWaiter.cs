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
                                           TimeSpan defaultTimeout) : this(system, subscriber, defaultTimeout, new MessageConditionFactory<Task<IWaitResult>>()) {}

        public ExplicitMessagesWaiter(ActorSystem system, 
                                           IActorSubscriber subscriber,
                                           TimeSpan defaultTimeout, 
                                           MessageConditionFactory<Task<IWaitResult>> messageConditionFactory) : base(system, subscriber, defaultTimeout, messageConditionFactory)
        {
            messageConditionFactory.CreateResultFunc = Start;
        }
    }
}