using System;

namespace GridDomain.EventSourcing
{
    public class Metadata : IMetadata
    {
        public Metadata(Guid id, Guid casuationId, Guid correlationId, ProcessHistory history = null)
        {
            Id = id;
            CasuationId = casuationId;
            CorrelationId = correlationId;
            History = new ProcessHistory(history);
        }

        public static Metadata CreateFrom(IMetadata existedMessage, Guid? id = null)
        {
            var metadata = new Metadata(id ?? Guid.NewGuid(), existedMessage.Id, existedMessage.CorrelationId, existedMessage.History);
            return metadata;
        }

        public Guid Id { get; }
        public Guid CasuationId { get; }
        public Guid CorrelationId { get; }
        public ProcessHistory History { get;} 
    }
}