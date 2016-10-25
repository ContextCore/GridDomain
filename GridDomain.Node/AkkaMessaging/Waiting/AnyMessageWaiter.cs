using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AnyMessageWaiter : MessageWaiter<ExpectedMessage>
    {
        protected override bool WaitIsOver(object message, ExpectedMessage expect)
        {
            return ReceivedMessagesHistory.Values.All(h => h.Received.Count >= h.Expected.MessageCount);
        }

        public AnyMessageWaiter(IActorRef subscribers, params ExpectedMessage[] expectedMessages) : base(subscribers, expectedMessages)
        {
        }


    }
}