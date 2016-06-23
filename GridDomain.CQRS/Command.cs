using System;

namespace GridDomain.CQRS
{
    public class Command : ICommand
    {
        protected Command(Guid id, DateTime time)
        {
            Id = id;
            Time = time;
        }

        protected Command(Guid id) : this(id, DateTime.UtcNow)
        {
        }

        protected Command() : this(Guid.NewGuid(), DateTime.UtcNow)
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