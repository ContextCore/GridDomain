using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Tests.Framework
{
    public class MessageWaiter : UntypedActor
    {
        private readonly Dictionary<Type, int> _messageCounters;
        private readonly IActorRef _notifyActor;
        private readonly List<object> _allReceivedEvents;

        public MessageWaiter(MessageToWait[] messagesToWait, IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
            _messageCounters = messagesToWait.ToDictionary(m => m.MessageType, m => m.Count);
            _allReceivedEvents = new List<object>();
        }

        protected override void OnReceive(object message)
        {
            var type = message.GetType();
            _allReceivedEvents.Add(message);

            if (!_messageCounters.ContainsKey(type)) return;
            if (--_messageCounters[type] > 0) return;

            _notifyActor.Tell(new ExpectedMessagesRecieved(message, _allReceivedEvents));
        }
    }
}