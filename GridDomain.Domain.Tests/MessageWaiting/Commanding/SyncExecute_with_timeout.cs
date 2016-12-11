using System;
using System.Linq;
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
    public class SyncExecute_with_timeout : SampleDomainCommandExecutionTests
    {
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(), CreateMap(), () => new[] {akkaConf.CreateInMemorySystem()},
                new InMemoryQuartzConfig());
        }
        [Then]
        public void CommandWaiter_doesnt_throw_exception_after_wait_with_timeout()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            GridNode.NewCommandWaiter(TimeSpan.FromMilliseconds(500))
                    .Expect<SampleAggregateChangedEvent>(e => e.SourceId == syncCommand.AggregateId)
                    .Create()
                    .Execute(syncCommand)
                    .Wait(100);
        }
    }
}