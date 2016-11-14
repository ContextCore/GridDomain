using System.Collections.Generic;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class WaitResults : IWaitResults
    {
        public WaitResults(IReadOnlyCollection<object> allReceivedMessages)
        {
            All = allReceivedMessages;
        }
        public IReadOnlyCollection<object> All { get; }
    }
}