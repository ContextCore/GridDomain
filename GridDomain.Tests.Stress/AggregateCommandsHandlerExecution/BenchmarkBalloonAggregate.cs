using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class BenchmarkBalloonAggregate : Aggregate
    {
        public string Title { get; private set; }
        protected BenchmarkBalloonAggregate(string id) : base(id)
        {
            
        }
        public BenchmarkBalloonAggregate(string id, string title):this(id)
        {
            Produce(new BalloonCreated(title,id));
        }

        public void WriteTitle(string newTitle)
        {
            Produce(new BalloonTitleChanged(newTitle, Id));
        }

        protected override void OnAppyEvent(DomainEvent evt)
        {
            switch(evt)
            {
                case BalloonTitleChanged e:
                    Title = e.Value;
                    break;
                case BalloonCreated e:
                    Id = e.Id;
                    Title = e.Value;
                    break;
            }
        }
    }
}