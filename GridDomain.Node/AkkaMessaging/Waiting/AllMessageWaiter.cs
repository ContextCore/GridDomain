using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AllMessageWaiter : MessageWaiter<ExpectedMessage>
    {
        protected override bool CanContinue(Dictionary<Type, int> messageCounters)
        {
            return messageCounters.Any(c => c.Value > 0);
        }

        public AllMessageWaiter(IActorRef notifyActor, params ExpectedMessage[] expectedMessages) : base(notifyActor, expectedMessages)
        {
        }
    }
}