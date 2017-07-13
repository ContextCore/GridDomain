using System;
using GridDomain.Common;
using GridDomain.Node.Actors.Hadlers;

namespace GridDomain.Tests.Unit.CommandPipe {
    class MarkedHandlerExecutedMessage : HandlerExecuted
    {
        public string Mark { get; }
        public MarkedHandlerExecutedMessage(string mark, IMessageMetadataEnvelop processingMessage, Exception error = null) : base(processingMessage, error)
        {
            Mark = mark;
        }
    }
}