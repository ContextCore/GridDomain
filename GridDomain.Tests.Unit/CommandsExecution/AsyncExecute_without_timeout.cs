using System;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class AsyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(),CreateMap(), () => new[]{akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig());
        }

        [Then]
        public async Task CommandWaiter_throws_exception_after_wait_with_only_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000,Guid.NewGuid());
            var waiter = GridNode.NewCommandWaiter(TimeSpan.FromMilliseconds(100))
                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                .Create()
                .Execute(syncCommand);

            await waiter.ShouldThrow<TimeoutException>();
        }


        [Then]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            GridNode.DefaultTimeout = TimeSpan.FromMilliseconds(100);
            var waiter = GridNode.NewCommandWaiter()
                .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                .Create()
                .Execute(syncCommand);

            await waiter.ShouldThrow<TimeoutException>();
        }


    }
}