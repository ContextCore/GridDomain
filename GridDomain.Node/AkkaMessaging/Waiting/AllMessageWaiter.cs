using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AllMessageWaiter : MessageWaiter<ExpectedMessage>
    {
        protected override bool WaitIsOver(object message, ExpectedMessage expect)
        {
            return ReceivedMessagesHistory.Values.All(h => h.Received.Count >= h.Expected.MessageCount);
        }

        public AllMessageWaiter(IActorRef notifyActor, params ExpectedMessage[] expectedMessages) : base(notifyActor, expectedMessages)
        {
        }
    }
}