using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiter
    {
        IReadOnlyCollection<object> AllReceivedMessages { get; }
        Task WaitAll();
        TMsg Received<TMsg>(Predicate<TMsg> selector = null);
    }
}