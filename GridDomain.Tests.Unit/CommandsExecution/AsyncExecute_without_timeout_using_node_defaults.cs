using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class AsyncExecute_without_timeout_using_node_defaults : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(), CreateMap(), () => new[] { akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig(), TimeSpan.FromMilliseconds(100));
        }

        [Then]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            await GridNode.PrepareCommand(syncCommand)
                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                .Execute()
                .ShouldThrow<TimeoutException>();
        }

    }
}