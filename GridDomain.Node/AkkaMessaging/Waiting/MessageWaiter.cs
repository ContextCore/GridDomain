using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
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
            var waitsForEventWithId = !string.IsNullOrEmpty(wait.IdPropertyName);

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