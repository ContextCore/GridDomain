using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessagesWaiterBuilder<TWaiter> where TWaiter: IMessageWaiter
    {
        IMessagesWaiterBuilder<TWaiter> Message<T>(Predicate<T> filter = null);
        IMessagesWaiterBuilder<TWaiter> Fault<T>(Predicate<IFault<T>> filter = null);
        TWaiter Create();
    }
}