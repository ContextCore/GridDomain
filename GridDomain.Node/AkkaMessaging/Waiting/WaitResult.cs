using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class WaitResult : IWaitResult
    {
        public WaitResult(IReadOnlyCollection<object> allReceivedMessages)
        {
            All = allReceivedMessages;
        }

        public IReadOnlyCollection<object> All { get; }
    }

    public class WaitResult<T> : WaitResult, IWaitResult<T> where T : class
    {
        public WaitResult(IMessageMetadataEnvelop<T> message, IMessageMetadataEnvelop<IFault> fault = null):base(new object[]{ message})
        {
            Received = message?.Message;
            Fault = fault?.Message;
            ReceivedMetadata = message?.Metadata ?? fault?.Metadata;
        }

        public T Received { get; }
        public IMessageMetadata ReceivedMetadata { get; }
        public IFault Fault { get; }
    }

    public static class WaitResultExtensions
    {
        public static async Task<T> Received<T>(this Task<WaitResult<T>> res) where T : class
        {
            return (await res).Received;
        }
    }
}