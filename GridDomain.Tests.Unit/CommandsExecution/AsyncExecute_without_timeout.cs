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
            var waiter = GridNode.Prepare(syncCommand)
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                                 .Execute(TimeSpan.FromMilliseconds(100));

            await waiter.ShouldThrow<TimeoutException>();
        }
    }
}