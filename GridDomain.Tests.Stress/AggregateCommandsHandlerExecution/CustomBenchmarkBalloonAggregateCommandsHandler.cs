using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Commands;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    class CustomBenchmarkBalloonAggregateCommandsHandler : IAggregateCommandsHandler<BenchmarkBalloonAggregate>
    {
        public async Task<BenchmarkBalloonAggregate> ExecuteAsync(BenchmarkBalloonAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            switch (command)
            {
                case WriteTitleCommand c:aggregate.WriteTitle(c.Parameter.ToString());
                    break;
                case InflateNewBallonCommand c: 
                    aggregate = new BenchmarkBalloonAggregate(c.AggregateId, c.Title.ToString());
                    aggregate.SetPersistProvider(persistenceDelegate);
                    break;
            }

            //for cases when we call Produce and expect events persistence after aggregate methods invocation;
            await persistenceDelegate(aggregate);
            return aggregate;
        }

        public IReadOnlyCollection<Type> RegisteredCommands => KnownCommands;
        private static readonly Type[] KnownCommands = { typeof(InflateNewBallonCommand), typeof(WriteTitleCommand) };
        public Type AggregateType { get; } = typeof(BenchmarkBalloonAggregate);
    }
}