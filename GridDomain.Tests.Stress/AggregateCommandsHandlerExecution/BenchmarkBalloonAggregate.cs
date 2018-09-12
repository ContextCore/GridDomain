using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Pattern;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
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
            Emit(new BalloonCreated(title,id));
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

        public override async Task<IReadOnlyCollection<DomainEvent>> Execute(ICommand command)
        {
            switch (command)
            {
                    case InflateNewBallonCommand c:
                        Emit(new BalloonCreated(c.Title.ToString(), Id));
                        break;
                    case WriteTitleCommand c:
                        WriteTitle(c.Parameter.ToString());
                        break;
            }

            return await Task.FromResult(_uncommittedEvents);
        }
    }
}