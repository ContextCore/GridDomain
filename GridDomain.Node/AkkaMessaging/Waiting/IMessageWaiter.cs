using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiter
    {
        Task<IWaitResults> ReceiveAll();
        Task<TMsg> Receive<TMsg>(Predicate<TMsg> filter = null);
    }

    public interface IWaitResults
    {
         IReadOnlyCollection<object> All { get; }
    }

    public static class WaitResultsExtensions
    {
        public static TMsg Message<TMsg>(this IWaitResults res, Predicate<TMsg> selector = null)
        {
            var sel = selector ?? (m => true);
            return res.All.OfType<TMsg>().FirstOrDefault(t => sel(t));
        }
    }
    public class WaitResults : IWaitResults
    {
        public WaitResults(IReadOnlyCollection<object> allReceivedMessages)
        {
            All = allReceivedMessages;
        }
        public IReadOnlyCollection<object> All { get; }
    }
}