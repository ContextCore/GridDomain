using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public abstract class Command : ICommand
    {
        protected Command(Guid id, Guid aggregateId, Guid sagaId, DateTime time)
        {
            Id = id;
            Time = time;
            SagaId = sagaId;
            AggregateId = aggregateId;
        }

        protected Command(Guid id, Guid aggregateId, Guid sagaId) : this(id, aggregateId, sagaId, BusinessDateTime.UtcNow)
        {
        }

        protected Command(Guid id, Guid aggregateId, DateTime time) : this(id, aggregateId, Guid.Empty, time)
        {
        }

        protected Command(Guid id, Guid aggregateId) : this(id, aggregateId, BusinessDateTime.UtcNow)
        {
        }

        protected Command(Guid aggregateId) : this(Guid.NewGuid(), aggregateId)
        {
        }

        public DateTime Time { get; private set; }
        public Guid Id { get; private set;}
        public Guid SagaId { get; private set; }
        public Guid AggregateId { get; private set; }

        public Command CloneWithSaga(Guid sagaId)
        {
            var copy = (Command)MemberwiseClone();
            copy.SagaId = sagaId;
            return copy;
        }
    }
}