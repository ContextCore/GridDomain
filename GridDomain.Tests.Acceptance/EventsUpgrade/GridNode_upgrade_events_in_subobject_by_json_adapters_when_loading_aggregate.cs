using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    public class GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate : NodeTestKit
    {
        public GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate(ITestOutputHelper output)
            : base(new BalloonFixture(output).UseSqlPersistence()
                                             .UseAdaper(new String01Adapter())
                                             .Configure(c => c.Log(LogEventLevel.Verbose,null,true)))
                                              { }

        private class String01Adapter : ObjectAdapter<string, string>
        {
            public override string Convert(string value)
            {
                return value + "01";
            }
        }

        [Fact]
        public async Task Then_domain_events_should_be_upgraded_by_json_custom_adapter()
        {
            var cmd = new InflateNewBallonCommand(1,
                                                  Guid.NewGuid().ToString());

            await Node.Prepare(cmd)
                      .Expect<BalloonCreated>()
                      .Execute();

            var aggregate = await Node.LoadAggregate<Balloon>(cmd.AggregateId);

            Assert.Equal("101", aggregate.Title);
        }
    }
}