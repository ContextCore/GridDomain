using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class AsyncExecute_without_timeout_using_node_defaults : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(), CreateMap(), () => new[] { akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig(), TimeSpan.FromMilliseconds(100));
        }

       [Fact]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            await Node.Prepare(syncCommand)
                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                .Execute()
                .ShouldThrow<TimeoutException>();
        }

    }
}