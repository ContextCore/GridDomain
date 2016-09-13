using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandWaiter : MessageWaiter<ExpectedMessage>
    {
        private readonly ICommand _command;

        public CommandWaiter(IActorRef notifyActor, ICommand command, params ExpectedMessage[] expectedMessage) : base(notifyActor, expectedMessage)
        {
            _command = command;
        }

        //execution stops on first expected fault
        protected override bool WaitIsOver(object message)
        {
            return IsExpectedFault(message)
                 //message faults are not counted while waiting for messages
                 || MessageReceivedCounters.All(c => !(typeof(IMessageFault).IsAssignableFrom(c.Key) && c.Value == 0));
        }

        //message is fault caller wish to know about
        private bool IsExpectedFault(object message)
        {
            var fault = message as IMessageFault;
            if (fault != null)
            {
                ExpectedMessage expectedFault;
                var type = message.GetType();
                if (ExpectedMessages.TryGetValue(type, out expectedFault))
                {
                    if (expectedFault.Source == fault.Processor)
                        return true;
                }
            }
            return false;
        }

        protected override object BuildAnswerMessage(object message)
        {
            object answerMessage = null;
            message.Match()
                   .With<IMessageFault>(f => answerMessage = f)
                   .Default(m =>
                   {
                       answerMessage = new CommandExecutionFinished(_command, m);
                   });

            return answerMessage;
        }
    }
}