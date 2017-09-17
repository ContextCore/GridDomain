using System;

namespace GridDomain.Common
{
    public class MessageMetadata : IMessageMetadata
    {
        public MessageMetadata(Guid messageId,
                               Guid? correlationId = null,
                               Guid? casuationId = null,
                               ProcessHistory history = null)
        {
            MessageId = messageId;
            CasuationId = casuationId ?? Guid.Empty;
            CorrelationId = correlationId ?? Guid.Empty;
            History = history ?? new ProcessHistory(null);
        }


        public static MessageMetadata Empty { get; } = new MessageMetadata(Guid.Empty);

        public Guid MessageId { get; }
        public Guid CasuationId { get; }
        public Guid CorrelationId { get; }
        public ProcessHistory History { get; }

        public static MessageMetadata CreateFrom(Guid messageId,
                                                 IMessageMetadata existedMessage,
                                                 params ProcessEntry[] process)
        {
            return new MessageMetadata(messageId,
                existedMessage.CorrelationId,
                existedMessage.MessageId,
#if DEBUG
                
                new ProcessHistory(process)
#endif
#if RELEASE
                null
#endif
            );

        }
    }
}