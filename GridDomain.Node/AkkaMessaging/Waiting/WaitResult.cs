using System;
using System.Collections.Generic;
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

    public class WaitResult<T> : WaitResult,IWaitResult<T>
    {
        public WaitResult(IMessageMetadataEnvelop<T> message ):base(new object[]{ message})
        {
            Received = message.Message;
            ReceivedMetadata = message.Metadata;
        }

        public T Received { get; }
        public IMessageMetadata ReceivedMetadata { get; }
    }
}