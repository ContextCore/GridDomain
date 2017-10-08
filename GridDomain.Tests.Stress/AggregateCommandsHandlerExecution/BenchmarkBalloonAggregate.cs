using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class BenchmarkBalloonAggregate : Aggregate
    {
        public string Title { get; private set; }
        protected BenchmarkBalloonAggregate(Guid id) : base(id)
        {
            
        }
        public BenchmarkBalloonAggregate(Guid id, string title):this(id)
        {
            Produce(new BalloonCreated(title,id));
        }

        public void WriteTitle(string newTitle)
        {
            Produce(new BalloonTitleChanged(newTitle, Id));
        }

        public override void ApplyEvent(DomainEvent @event)
        {
            switch (@event)
            {
                case BalloonTitleChanged e:
                    Title = e.Value;
                    break;
                case BalloonCreated e:
                    Id = e.Id;
                    Title = e.Value;
                    break;
            }
            base.ApplyEvent(@event);
        }
    }
}