using System;
using GridDomain.CQRS.Messaging;
using MemBus;

namespace GridDomain.Node.MemTransportMessaging
{
    public class MemTransportToMessageTransportAdapter : IMessageTransport
    {
        private readonly IBus _memBus;

        public MemTransportToMessageTransportAdapter(IBus memBus)
        {
            _memBus = memBus;
        }

        public void Subscribe<TMessage>(Action<TMessage> msgHandler)
        {
            _memBus.Subscribe(msgHandler);
        }

        public void Publish<T>(T msg)
        {
            _memBus.Publish(msg);
        }
    }
}