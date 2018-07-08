using System;
using System.Collections.Generic;
using System.Linq;
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

        public static WaitResult<T> Parse<T>(IWaitResult res) where T : class
        {
            var expectedTypedMessage = res.All.OfType<IMessageMetadataEnvelop>().ToArray();
            if(expectedTypedMessage.Length > 1) throw new InvalidOperationException("Too many results");


            return new WaitResult<T>(expectedTypedMessage.FirstOrDefault());
        }
    }

    public class WaitResult<T> : WaitResult, IWaitResult<T> where T : class
    {
        public WaitResult(IMessageMetadataEnvelop message):base(new object[]{ message})
        {
            Received = message?.Message as T;
            ReceivedMetadata = message?.Metadata;
        }

        public T Received { get; }
        public IMessageMetadata ReceivedMetadata { get; }
    }

   
}