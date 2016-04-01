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

        protected Command() : this(Guid.NewGuid(), DateTime.UtcNow)
        {
        }

        public DateTime Time { get; private set; }
        public Guid Id { get; }
        public Guid SagaId { get; set; }
    }
}