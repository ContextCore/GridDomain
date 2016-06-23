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

        public MessageWaiter(MessageToWait[] messagesToWait, IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
            _messageCounters = messagesToWait.ToDictionary(m => m.MessageType, m => m.Count);
        }

        protected override void OnReceive(object message)
        {
            var type = message.GetType();
            if (!_messageCounters.ContainsKey(type)) return;
            if (--_messageCounters[type] > 0) return;

            _notifyActor.Tell(new ExpectedMessagesRecieved(message));
        }
    }
}