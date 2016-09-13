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
        private readonly List<object> _allReceivedEvents;

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
            var type = message.GetType();
            _allReceivedEvents.Add(message);

            if (!MessageReceivedCounters.ContainsKey(type)) return;

            var wait = ExpectedMessages[type];
            if (!wait.Like(message)) return;

            --MessageReceivedCounters[type];
            if (!WaitIsOver(message)) return;

            _notifyActor.Tell(BuildAnswerMessage(message));
        }

        protected abstract bool WaitIsOver(object message);
        protected virtual object BuildAnswerMessage(object message)
        {
            return new ExpectedMessagesRecieved(message, _allReceivedEvents);
        }
    }
}