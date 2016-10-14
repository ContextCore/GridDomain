using System;
using Akka.Actor;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class AllMessagesWaiter : LocalMessageWaiter
    {
        public AllMessagesWaiter(ActorSystem system, TimeSpan timeout) 
            : base(CreateWaiter(system),timeout)
        {
        }

        private static IActorRef CreateWaiter(ActorSystem system)
        {
            var props = Props.Create(() => new AllMessageWaiterActor(null, null, null));
            return system.ActorOf(props, "MessagesWaiter_" + Guid.NewGuid());
        }
    }
}