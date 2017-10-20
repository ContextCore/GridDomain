using System;

namespace GridDomain.Common
{
    public class MessageMetadata : IMessageMetadata
    {
        public MessageMetadata(string messageId,
                               string correlationId = null,
                               string casuationId = null,
                               ProcessHistory history = null)
        {
            MessageId = messageId;
            CasuationId = casuationId;
            CorrelationId = correlationId;
            History = history ?? new ProcessHistory(null);
        }

        public MessageMetadata(Guid messageId,
                               Guid? correlationId = null,
                               Guid? casuationId = null,
                               ProcessHistory history = null):this(messageId.ToString("D"),
                                                                   correlationId?.ToString("D"),
                                                                   casuationId?.ToString("D"),
                                                                   history)
        {
        }

        public static MessageMetadata Empty { get; } = new MessageMetadata("","");

        public string MessageId { get; }
        public string CasuationId { get; }
        public string CorrelationId { get; }
        public ProcessHistory History { get; }

        public static MessageMetadata CreateFrom(Guid messageId,
                                                 IMessageMetadata existedMessage,
                                                 params ProcessEntry[] process)
        {
            return new MessageMetadata(messageId.ToString("D"),
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