using System.Collections.Generic;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IWaitResult
    {
        IReadOnlyCollection<object> All { get; }
    }
    public interface IWaitResult<out T> : IWaitResult
    {
        T Received { get; }
        IFault Fault { get; }
        IMessageMetadata ReceivedMetadata { get; }
    }
}