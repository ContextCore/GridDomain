using System;

namespace GridDomain.CQRS
{
    public class Command : ICommand
    {
        public Guid Id { get; private set; }
        public DateTime Time { get; private set; }
        public Guid SagaId { get; set; }

        protected Command(Guid id, DateTime time)
        {
            Id = id;
            Time = time;
        }

        protected Command() : this(Guid.NewGuid(), DateTime.UtcNow)
        {
            
        }
    }
}