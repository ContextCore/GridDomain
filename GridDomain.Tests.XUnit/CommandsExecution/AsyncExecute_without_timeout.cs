using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class AsyncExecute_without_timeout : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(),CreateMap(), () => new[]{akkaConf.CreateInMemorySystem() },
                new InMemoryQuartzConfig());
        }

       [Fact]
        public async Task CommandWaiter_throws_exception_after_wait_with_only_default_timeout()
        {
            var syncCommand = new LongOperationCommand(1000,Guid.NewGuid());
            var waiter =Node.Prepare(syncCommand)
                                 .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                                 .Execute(TimeSpan.FromMilliseconds(100));

            await waiter.ShouldThrow<TimeoutException>();
        }
    }
}