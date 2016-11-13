using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
{
    [TestFixture]
    public class SyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(),CreateMap(), () => new[]{akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig());
        }

        [Then]
        public async Task CommandWaiter_throws_exception_after_wait_with_obly_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000,Guid.NewGuid());
            var waiter = GridNode.NewCommandWaiter(TimeSpan.FromMilliseconds(100))
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                                 .Create()
                                 .Execute(syncCommand);

            await AssertEx.ShouldThrow<TimeoutException>(waiter);
        }

        [Then]
        public async Task SyncExecute_throw_exception_after_wait_without_timeout()
        {
            var syncCommand = new LongOperationCommand(1000000, Guid.NewGuid());
            var waiter = GridNode.NewCommandWaiter()
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                                 .Create()
                                 .Execute(syncCommand);

            await waiter.ShouldThrow<TimeoutException>();
            //default built-in timeout for 10 sec
        }

        [Then]
        public async Task SyncExecute_throw_exception_according_to_node_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000000, Guid.NewGuid());
            GridNode.DefaultTimeout = TimeSpan.FromMilliseconds(500);
            var waiter = GridNode.NewCommandWaiter()
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                                 .Create()
                                 .Execute(syncCommand);

            await AssertEx.ShouldThrow<TimeoutException>(waiter);
        }

        [Then]
        public void CommandWaiter_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            GridNode.NewCommandWaiter()
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                                 .Create(TimeSpan.FromMilliseconds(500))
                                 .Execute(syncCommand)
                                 .Wait(100);
        }
    }
}