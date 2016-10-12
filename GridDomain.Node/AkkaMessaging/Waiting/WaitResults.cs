using System.Collections.Generic;

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