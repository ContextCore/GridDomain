using System.Collections.Generic;

namespace GridDomain.CQRS
{
    public interface IWaitResults
    {
        IReadOnlyCollection<object> All { get; }
    }
}