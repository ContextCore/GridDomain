using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class BenchmarkBallonCommandAggregate:CommandAggregate
    {
        public string Title { get; private set; }
        protected BenchmarkBallonCommandAggregate(Guid id) : base(id)
        {

        }
        public BenchmarkBallonCommandAggregate(Guid id, string title):this(id)
        {
            Produce(new BalloonCreated(title, id));
        }

        public void WriteTitle(string newTitle)
        {
            Produce(new BalloonTitleChanged(newTitle, Id));
        }
        
        public override IReadOnlyCollection<Type> RegisteredCommands => KnownCommands;
        private static readonly Type[] KnownCommands = {typeof(InflateNewBallonCommand), typeof(WriteTitleCommand)};
        protected override Task<IAggregate> Execute(ICommand cmd)
        {
            switch (cmd)
            {
                case InflateNewBallonCommand c:
                    return Task.FromResult<IAggregate>(new BenchmarkBallonCommandAggregate(c.AggregateId, c.Title.ToString()));
                case WriteTitleCommand c:
                    WriteTitle(c.Parameter.ToString());
                    break;
            }
            return Task.FromResult<IAggregate>(this);
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