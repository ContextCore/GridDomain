using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Tests.Framework;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandAndConfirmation
    {
        public TimeSpan Timeout { get; }
        public ExpectedMessage ExpectedMessage { get; }
        public ICommand Command { get; }

        public CommandAndConfirmation(ICommand command, ExpectedMessage expectedMessage, TimeSpan timeout)
        {
            Timeout = timeout;
            ExpectedMessage = expectedMessage;
            Command = command;
        }
    }

    public class CommandWaiter : MessageWaiter
    {
        private readonly ICommand _command;

        public CommandWaiter(IActorRef notifyActor, ICommand command, ExpectedMessage expectedMessage) : base(notifyActor, expectedMessage)
        {
            _command = command;
        }

        protected override object BuildAnswerMessage(object message)
        {
            object answerMessage = null;
            message.Match()
                   .With<ICommandFault>(f => answerMessage = f)
                   .Default(m =>
                   {
                       answerMessage = new CommandExecutionFinished(_command, m);
                   });

            return answerMessage;
        }
    }

    public class MessageWaiter : MessageWaiter<ExpectedMessage>
    {
        public MessageWaiter(IActorRef notifyActor, params ExpectedMessage[] expectedMessages) : base(notifyActor, expectedMessages)
        {
        }
    }


    public class MessageWaiter<T> : UntypedActor where T: ExpectedMessage
    {
        protected readonly Dictionary<Type, int> MessageCounters;
        protected readonly Dictionary<Type, T> MessageWaits;
        private readonly IActorRef _notifyActor;
        private readonly List<object> _allReceivedEvents;

        public MessageWaiter(IActorRef notifyActor, params T[] expectedMessages)
        {
            _notifyActor = notifyActor;
            MessageCounters = expectedMessages.ToDictionary(m => m.MessageType, m => m.MessageCount);
            MessageWaits = expectedMessages.ToDictionary(m => m.MessageType, m => m);

            _allReceivedEvents = new List<object>();
        }

        /// <summary>
        /// Will count only messages of known type and with known Id, if IdPropertyName is specified
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            var type = message.GetType();
            _allReceivedEvents.Add(message);

            if (!MessageCounters.ContainsKey(type)) return;

            var wait = MessageWaits[type];
            var waitsForEventWithId = string.IsNullOrEmpty(wait.IdPropertyName);

            if (waitsForEventWithId)
            {
                var messageId = type.GetProperty(wait.IdPropertyName).GetValue(message);
                if (wait.MessageId != (Guid) messageId)return;
            }

            --MessageCounters[type];
            if (MessageCounters.Any(c => c.Value > 0)) return;
                
            _notifyActor.Tell(BuildAnswerMessage(message));
        }

        protected virtual object BuildAnswerMessage(object message)
        {
            return new ExpectedMessagesRecieved(message, _allReceivedEvents);
        }
    }
}