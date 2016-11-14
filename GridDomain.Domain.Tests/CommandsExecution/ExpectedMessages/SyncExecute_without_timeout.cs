using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.MessageWaiting.Commanding;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class CommandPlan_Execute_without_timeout : InMemorySampleDomainTests
    {

        [Then]
        public async Task PlanExecute_throw_exception_after_wait_without_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, TimeSpan.FromMilliseconds(100), expectedMessage);

            await GridNode.Execute(plan)
                          .ShouldThrow<TimeoutException>();
        }

    }



    [TestFixture]
    public class SyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {

        public SyncExecute_without_timeout() : base(true)
        {

        }

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(),CreateMap(), () => new[]{akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig());
        }

        [Then]
        public async Task PlanExecute_by_result_throws_exception_after_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, TimeSpan.FromMilliseconds(500), expectedMessage);

            await GridNode.Execute(plan)
                          .ShouldThrow<TimeoutException>();
        }
        
        [Then]
        public void PlanExecute_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var plan = CommandPlan.New(syncCommand, expectedMessage);

            Assert.False(GridNode.Execute(plan).Wait(100));
        }
    }
}