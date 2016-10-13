using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiter
    {
        //IActorRef Receiver { get; }
        Task<IWaitResults> ReceiveAll();
        Task<T> ReceiveAll<T>();
        Task<TMsg> Receive<TMsg>(Predicate<TMsg> filter = null);
    }
}