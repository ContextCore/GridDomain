using System;

namespace GridDomain.Common
{
    public class MessageMetadata : IMessageMetadata
    {
        public MessageMetadata(Guid messageId, Guid? correlationId = null, Guid? casuationId = null, ProcessHistory history = null)
        {
            MessageId = messageId;
            CasuationId = casuationId ?? Guid.Empty;
            CorrelationId = correlationId ?? Guid.Empty;
            History = new ProcessHistory(history);
        }

        public static MessageMetadata CreateFrom(IMessageMetadata existedMessage, Guid? id = null)
        {
            var metadata = new MessageMetadata(id ?? Guid.NewGuid(), correlationId: existedMessage.CorrelationId, casuationId: existedMessage.MessageId, history: existedMessage.History);
            return metadata;
        }

        public Guid MessageId { get; }
        public Guid CasuationId { get; }
        public Guid CorrelationId { get; }
        public ProcessHistory History { get;} 
    }
}