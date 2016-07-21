using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AnyMessageWaiter : MessageWaiter<ExpectedMessage>
    {
        protected override bool CanContinue(Dictionary<Type, int> messageCounters)
        {
            return messageCounters.All(c => c.Value > 0);
        }

        public AnyMessageWaiter(IActorRef notifyActor, params ExpectedMessage[] expectedMessages) : base(notifyActor, expectedMessages)
        {
        }
    }
}