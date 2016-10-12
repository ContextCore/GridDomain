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
        private readonly List<IActorRef> _resultMessageSubscribers = new List<IActorRef>();
        private readonly Dictionary<Type,List<IActorRef>> _receivedMessageSubscribers = new Dictionary<Type, List<IActorRef>>();
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
            _resultMessageSubscribers.Add(subscriber);

            ReceivedMessagesHistory = expectedMessages.ToDictionary(m => m.MessageType, 
                                                                    m => new ExpectedMessageHistory(m));
        }

        /// <summary>
        /// Will count only messages of known type and with known Id, if IdPropertyName is specified
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object msg)
        {
            msg.Match()
                .With<NotifyOnWaitEnd>(s => { _resultMessageSubscribers.Add(Sender); })
                .With<NotifyOnMessage>(notify =>
                {
                    List<IActorRef> list;
                    if (!_receivedMessageSubscribers.TryGetValue(notify.MessageType, out list))
                    {
                        list = new List<IActorRef>();
                        _receivedMessageSubscribers[notify.MessageType] = list;
                    }
                    list.Add(Sender);
                })
                .Default(message =>
                {
                    var history = FindHistory(message);
                    if(history == null) return;
                    
                    NotifyMessageSubscribers(history.Expected.MessageType, message);

                    if (!history.Expected.Match(message)) return;
                    history.Received.Add(message);

                    if (!WaitIsOver(message, history.Expected)) return;

                    var answerMessage = BuildAnswerMessage(message);

                    NotifyWaitSubscribers(answerMessage);

                    Context.Stop(Self);
                });
        }

        private ExpectedMessageHistory FindHistory(object message)
        {
            var msgType = message.GetType();
            var registeredType = ReceivedMessagesHistory.Keys.FirstOrDefault(k => k.IsAssignableFrom(msgType));
            return registeredType == null ? null : ReceivedMessagesHistory[registeredType];
        }

        private void NotifyMessageSubscribers(Type msgType, object message)
        {
            List<IActorRef> msgSubscribers;
            if (_receivedMessageSubscribers.TryGetValue(msgType, out msgSubscribers))
                NotifyWaitSubscribers(message);
        }

        private void NotifyWaitSubscribers(object answerMessage)
        {
            foreach (var s in _resultMessageSubscribers)
                 s.Tell(answerMessage);
        }

        protected abstract bool WaitIsOver(object message, ExpectedMessage expect);
        protected virtual object BuildAnswerMessage(object message)
        {
            return new ExpectedMessagesReceived(message, ReceivedMessagesHistory.Values.SelectMany(v => v.Received).ToArray());
        }
    }
}