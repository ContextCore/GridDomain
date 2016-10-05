using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public abstract class MessageWaiter<T> : UntypedActor where T : ExpectedMessage
    {
        private readonly IActorRef _notifyActor;
        protected readonly Dictionary<Type, ExpectedMessageHistory> ReceivedMessagesHistory;

        protected class ExpectedMessageHistory
        {
            public List<object> Received { get; } = new List<object>();
            public T Expected { get; }

            public ExpectedMessageHistory(T expected)
            {
                Expected = expected;
            }
        }

        protected MessageWaiter(IActorRef notifyActor, params T[] expectedMessages)
        {
            _notifyActor = notifyActor;
            ReceivedMessagesHistory = expectedMessages.ToDictionary(m => m.MessageType, 
                                                             m => new ExpectedMessageHistory(m));
        }

        /// <summary>
        /// Will count only messages of known type and with known Id, if IdPropertyName is specified
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            var msgType = message.GetType();
          
            var registeredType = ReceivedMessagesHistory.Keys.FirstOrDefault(k => k.IsAssignableFrom(msgType));
            if (registeredType == null) return;

            var history = ReceivedMessagesHistory[registeredType];

            var expect = history.Expected;
            if (!expect.Match(message)) return;

            history.Received.Add(message);

            if (!WaitIsOver(message, expect)) return;

            _notifyActor.Tell(BuildAnswerMessage(message));
        }

        protected abstract bool WaitIsOver(object message, ExpectedMessage expect);
        protected virtual object BuildAnswerMessage(object message)
        {
            return new ExpectedMessagesRecieved(message, ReceivedMessagesHistory.Values.SelectMany(v => v.Received).ToArray());
        }
    }
}