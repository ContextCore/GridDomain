using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IMessageWaiterFactory
    {
        /// <summary>
        /// Wait for messages without metadata
        /// </summary>
        /// <param name="defaultTimeout"></param>
        /// <returns></returns>
        IMessageWaiter<Task<IWaitResults>> NewExplicitWaiter(TimeSpan? defaultTimeout = null);
        /// <summary>
        /// Wait for messages with metadata envelop, e.g. IMessageWithMetadata<T>
        /// </summary>
        /// <param name="defaultTimeout"></param>
        /// <returns></returns>
        IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null);
    }
}