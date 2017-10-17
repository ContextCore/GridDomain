using System.Text;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tools;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution
{

    class BenchmarkBalloonAggregateCommandsHandler : AggregateCommandsHandler<BenchmarkBalloonAggregate>
    {
        public BenchmarkBalloonAggregateCommandsHandler()
        {
            Map<WriteTitleCommand>((c,a) => a.WriteTitle(c.Parameter.ToString()));
            Map<InflateNewBallonCommand>(c => new BenchmarkBalloonAggregate(c.AggregateId,c.Title.ToString()));
        }
    }

#pragma warning disable xUnit1013
}
