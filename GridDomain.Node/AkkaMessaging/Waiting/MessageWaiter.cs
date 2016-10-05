using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public abstract class MessageWaiter<T> : UntypedActor where T : ExpectedMessage
    {
        protected readonly Dictionary<Type, int> MessageReceivedCounters;
        protected readonly Dictionary<Type, T> ExpectedMessages;
        private readonly IActorRef _notifyActor;
        protected readonly List<object> _allReceivedEvents;

        protected MessageWaiter(IActorRef notifyActor, params T[] expectedMessages)
        {
            _notifyActor = notifyActor;
            MessageReceivedCounters = expectedMessages.ToDictionary(m => m.MessageType, m => m.MessageCount);
            ExpectedMessages = expectedMessages.ToDictionary(m => m.MessageType, m => m);

            _allReceivedEvents = new List<object>();
        }

        /// <summary>
        /// Will count only messages of known type and with known Id, if IdPropertyName is specified
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            var msgType = message.GetType();
            _allReceivedEvents.Add(message);

            var registeredType = MessageReceivedCounters.Keys.FirstOrDefault(k => k.IsAssignableFrom(msgType));
            if (registeredType == null) return;

            var expect = ExpectedMessages[registeredType];
            if (!expect.Match(message)) return;

            --MessageReceivedCounters[registeredType];
            if (!WaitIsOver(message, expect)) return;

            _notifyActor.Tell(BuildAnswerMessage(message));
        }

        protected abstract bool WaitIsOver(object message, ExpectedMessage expect);
        protected virtual object BuildAnswerMessage(object message)
        {
            return new ExpectedMessagesRecieved(message, _allReceivedEvents);
        }
    }
}