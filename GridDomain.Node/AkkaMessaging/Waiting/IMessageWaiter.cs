using System;
using System.Diagnostics;
using System.Security.Policy;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiter<T>
    {
        IExpectBuilder<T> Expect<TMsg>(Predicate<TMsg> filter = null);
    }
}