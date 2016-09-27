using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class Command : ICommand
    {
        protected Command(Guid id, Guid sagaId, DateTime time)
        {
            Id = id;
            Time = time;
            SagaId = sagaId;
        }

        protected Command(Guid id, Guid sagaId) : this(id, sagaId, BusinessDateTime.UtcNow)
        {
        }

        protected Command(Guid id, DateTime time) : this(id, Guid.Empty, time)
        {
        }

        protected Command(Guid id) : this(id, BusinessDateTime.UtcNow)
        {
        }

        protected Command() : this(Guid.NewGuid())
        {
        }

        public DateTime Time { get; }
        public Guid Id { get; }
        public Guid SagaId { get; private set; }

        public Command CloneWithSaga(Guid sagaId)
        {
            var copy = (Command)MemberwiseClone();
            copy.SagaId = sagaId;
            return copy;
        }
    }
}