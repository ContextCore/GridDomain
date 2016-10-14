using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandWaiterActor : MessageWaiterActor<ExpectedMessage>
    {
        public CommandWaiterActor(IActorRef subscribers = null, params ExpectedMessage[] expectedMessage) : base(subscribers, expectedMessage)
        {
        }

        //execution stops on first expected fault
        protected override bool WaitIsOver(object message,ExpectedMessage expect)
        {
            return IsExpectedFault(message, expect) || AllExpectedMessagesReceived();
        }

        private bool AllExpectedMessagesReceived()
        {
            //message faults are not counted while waiting for messages
            return ReceivedMessagesHistory.Where(c => !typeof(IFault).IsAssignableFrom(c.Key))
                                          .All(h => h.Value.Received.Count >= h.Value.Expected.MessageCount);
        }

        //message is fault that caller wish to know about
        //if no special processor type of fault is specified, we will stop on any fault
        private bool IsExpectedFault(object message, ExpectedMessage expect)
        {
            var fault = message as IFault;
            return fault != null && (!expect.Sources.Any() || expect.Sources.Contains(fault.Processor));
        }

        protected override object BuildAnswerMessage(object message)
        {
            if (message is IFault) return message;
            return base.BuildAnswerMessage(message);
        }
    }
}