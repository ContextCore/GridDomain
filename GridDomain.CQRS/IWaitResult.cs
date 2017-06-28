using System.Collections.Generic;
using System.Threading.Tasks;
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

    public static class WaitResultExtensions
    {
        public static async Task<T> Received<T>(this Task<IWaitResult<T>> res) where T : class
        {
            return (await res).Received;
        }
    }


}