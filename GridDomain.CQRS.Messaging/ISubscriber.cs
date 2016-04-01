using System;

namespace GridDomain.CQRS.Messaging
{
    public interface ISubscriber
    {
        void Subscribe<TMessage>(Action<TMessage> msgHandler);
    }


}