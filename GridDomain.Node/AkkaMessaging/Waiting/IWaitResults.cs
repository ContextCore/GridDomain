using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IWaitResults
    {
        IReadOnlyCollection<object> All { get; }
    }
}