using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    
    
    
    public class BenchmarkBallonCommandAggregate:ConventionAggregate
    {
        public string Title { get; private set; }
        protected BenchmarkBallonCommandAggregate(string id) : base(id)
        {
            Execute<InflateNewBallonCommand>(c => Emit(new BalloonCreated(c.Title.ToString(), id)));
            Execute<WriteTitleCommand>(c => WriteTitle(c.Parameter.ToString()));
        }
        public BenchmarkBallonCommandAggregate(string id, string title):this(id)
        {
            Emit(new BalloonCreated(title, id));
        }

        public void WriteTitle(string newTitle)
        {
            Emit(new BalloonTitleChanged(newTitle, Id));
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