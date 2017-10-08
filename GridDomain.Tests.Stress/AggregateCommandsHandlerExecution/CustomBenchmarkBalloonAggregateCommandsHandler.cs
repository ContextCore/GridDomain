using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    class CustomBenchmarkBalloonAggregateCommandsHandler : IAggregateCommandsHandler<BenchmarkBalloonAggregate>
    {
        public async Task<BenchmarkBalloonAggregate> ExecuteAsync(BenchmarkBalloonAggregate aggregate, ICommand command, IEventStore eventStore)
        {
            switch (command)
            {
                case WriteTitleCommand c:
                    aggregate.WriteTitle(c.Parameter.ToString());
                    break;
                case InflateNewBallonCommand c: 
                    aggregate = new BenchmarkBalloonAggregate(c.AggregateId, c.Title.ToString());
                    aggregate.InitEventStore(eventStore);
                    break;
                default:
                return aggregate;
            }

            await eventStore.Persist(aggregate);
            return aggregate;
        }

        public IReadOnlyCollection<Type> RegisteredCommands => KnownCommands;
        private static readonly Type[] KnownCommands = { typeof(InflateNewBallonCommand), typeof(WriteTitleCommand) };
        public Type AggregateType { get; } = typeof(BenchmarkBalloonAggregate);
    }
}