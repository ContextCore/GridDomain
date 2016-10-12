using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiter
    {
        Task<IWaitResults> ReceiveAll();
        Task<T> ReceiveAll<T>();
        Task<TMsg> Receive<TMsg>(Predicate<TMsg> filter = null);
    }
}