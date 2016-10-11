using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
 
    public abstract class MessageWaiterActor<T> : UntypedActor where T : ExpectedMessage
    {
        private readonly List<IActorRef> _subscribers = new List<IActorRef>();
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

        protected MessageWaiterActor(IActorRef subscriber = null, params T[] expectedMessages)
        {
            if(subscriber != null)
            _subscribers.Add(subscriber);

            ReceivedMessagesHistory = expectedMessages.ToDictionary(m => m.MessageType, 
                                                                    m => new ExpectedMessageHistory(m));
        }

        /// <summary>
        /// Will count only messages of known type and with known Id, if IdPropertyName is specified
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            if (message is NotifyOnWaitEnd)
            {
                _subscribers.Add(Sender);
                return;
            }

            var msgType = message.GetType();
          
            var registeredType = ReceivedMessagesHistory.Keys.FirstOrDefault(k => k.IsAssignableFrom(msgType));
            if (registeredType == null) return;

            var history = ReceivedMessagesHistory[registeredType];

            var expect = history.Expected;
            if (!expect.Match(message)) return;

            history.Received.Add(message);

            if (!WaitIsOver(message, expect)) return;

            var answerMessage = BuildAnswerMessage(message);

            foreach (var s in _subscribers)
                s.Tell(answerMessage);

            Context.Stop(Self);
        }

        protected abstract bool WaitIsOver(object message, ExpectedMessage expect);
        protected virtual object BuildAnswerMessage(object message)
        {
            return new ExpectedMessagesReceived(message, ReceivedMessagesHistory.Values.SelectMany(v => v.Received).ToArray());
        }
    }

    public class NotifyOnWaitEnd
    {
        public static NotifyOnWaitEnd Instance = new NotifyOnWaitEnd();
    }
}