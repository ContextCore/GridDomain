using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class AnyMessageWaiterActor : MessageWaiterActor<ExpectedMessage>
    {
        protected override bool WaitIsOver(object message, ExpectedMessage expect)
        {
            return ReceivedMessagesHistory.Values.All(h => h.Received.Count >= h.Expected.MessageCount);
        }

        public AnyMessageWaiterActor(IActorRef subscribers, params ExpectedMessage[] expectedMessages) : base(subscribers, expectedMessages)
        {
        }


    }
}