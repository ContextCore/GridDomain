using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class LocalMessagesWaiter : LocalMessagesWaiter<Task<IWaitResult>>
    {
        public LocalMessagesWaiter(ActorSystem system,
                                   IActorSubscriber subscriber,
                                   TimeSpan defaultTimeout) : this(system,
                                                                   subscriber,
                                                                   defaultTimeout,
                                                                   new MetadataConditionBuilder<Task<IWaitResult>>()) {}

        private LocalMessagesWaiter(ActorSystem system,
                                    IActorSubscriber subscriber,
                                    TimeSpan defaultTimeout,
                                    ConditionBuilder<Task<IWaitResult>> conditionBuilder) : base(system,
                                                                                                 subscriber,
                                                                                                 defaultTimeout,
                                                                                                 conditionBuilder)
        {
            conditionBuilder.CreateResultFunc = Start;
        }
    }
}