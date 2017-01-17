using System;
using GridDomain.Common;

namespace GridDomain.Node.Actors
{
    class HandlerExecuted
    {
        public HandlerExecuted(IMessageMetadataEnvelop processingMessage, Exception error = null)
        {
            ProcessingMessage = processingMessage;
            Error = error;
        }

        public IMessageMetadataEnvelop ProcessingMessage { get; }
        public Exception Error { get; }
    }
}