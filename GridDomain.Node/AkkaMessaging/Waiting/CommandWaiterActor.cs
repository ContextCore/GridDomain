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
        private readonly ICommand _command;

        public CommandWaiterActor(IActorRef subscribers, ICommand command, params ExpectedMessage[] expectedMessage) : base(subscribers, expectedMessage)
        {
            _command = command;
        }

        public CommandWaiterActor(IActorRef subscribers, CommandPlan plan) : base(subscribers, plan.ExpectedMessages)
        {
            _command = plan.Command;
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
            object answerMessage = null;
            message.Match()
                   .With<IFault>(f => answerMessage = f)
                   .Default(m =>
                   {
                       var allReceivedMessages = ReceivedMessagesHistory.Values.SelectMany(v => v.Received).ToArray();
                       answerMessage = new CommandExecutionFinished(_command,
                           allReceivedMessages.Length > 1 ? allReceivedMessages.ToArray() : m);
                   });

            return answerMessage;
        }
    }
}