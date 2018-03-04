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
        public Task<BenchmarkBalloonAggregate> ExecuteAsync(BenchmarkBalloonAggregate aggregate, ICommand command)
        {
            switch (command)
            {
                case WriteTitleCommand c:
                    aggregate.WriteTitle(c.Parameter.ToString());
                    break;
                case InflateNewBallonCommand c: 
                    aggregate = new BenchmarkBalloonAggregate(c.AggregateId, c.Title.ToString());
                    break;
                default:
                return Task.FromResult(aggregate);
            }
            return Task.FromResult(aggregate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands => KnownCommands;
        private static readonly Type[] KnownCommands = { typeof(InflateNewBallonCommand), typeof(WriteTitleCommand) };
        public Type AggregateType { get; } = typeof(BenchmarkBalloonAggregate);
    }
}