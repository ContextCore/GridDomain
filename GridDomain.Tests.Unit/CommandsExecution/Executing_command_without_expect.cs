using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Repositories.EventRepositories;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_awaiting_command_execution_without_prepare : BalloonDomainCommandExecutionTests
    {
        public When_awaiting_command_execution_without_prepare(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Then_command_executed_aggregate_is_persisted()
        {
            var aggregateId = Guid.NewGuid();
            await Node.Execute(new InflateCopyCommand(100, aggregateId),
                               new WriteTitleCommand(200, aggregateId));

            var aggregate = await Node.LoadAggregate<Balloon>(aggregateId);
            Assert.Equal("200", aggregate.Title);
        }
    }


    public class When_awaiting_command_execution_without_prepare_and_counting : NodeTestKit
    {
        class BalloonCountingDomainConfiguration : IDomainConfiguration
        {
            public void Register(IDomainBuilder builder)
            {
                builder.RegisterAggregate(new BalloonDependencyFactory());
                builder.RegisterHandler<BalloonCreated, CountingMessageHandler>().AsParallel();
                builder.RegisterHandler<BalloonCreated, SlowCountingMessageHandler>().AsFireAndForget();
                builder.RegisterHandler<BalloonTitleChanged, CountingMessageHandler>().AsSync();
            }
        }

        class CountingMessageHandler : IHandler<BalloonCreated>,
                                       IHandler<BalloonTitleChanged>
        {
            public static int CreatedCount;
            public static int ChangedCount;
            public Task Handle(BalloonCreated message, IMessageMetadata metadata = null)
            {
                CreatedCount++;
                return Task.Delay(50);
            }

            public Task Handle(BalloonTitleChanged message, IMessageMetadata metadata = null)
            {
                ChangedCount++;
                return Task.Delay(15);
            }
        }
        class SlowCountingMessageHandler : IHandler<BalloonCreated>
        {
            public static int CreatedCount;
            public async Task Handle(BalloonCreated message, IMessageMetadata metadata = null)
            {
                await Task.Delay(300);
                CreatedCount++;
            }
        }

        public When_awaiting_command_execution_without_prepare_and_counting(ITestOutputHelper output) :
            base(output, new NodeTestFixture(new BalloonCountingDomainConfiguration())){ }

        [Fact]
        public async Task Then_command_executed_sync_and_parralel_message_processor_are_executed()
        {
            var aggregateId = Guid.NewGuid();

            //will produce one created message and two title changed
            await Node.Execute(new InflateCopyCommand(100, aggregateId),
                               new WriteTitleCommand(200, aggregateId));

            Assert.Equal(1, CountingMessageHandler.CreatedCount);
            Assert.Equal(2, CountingMessageHandler.ChangedCount);
            //will not wait antil Fire and Forget handlers
            Assert.Equal(0, SlowCountingMessageHandler.CreatedCount);
            await Task.Delay(350);
            //but Fire and Forget handler was launched and will complete later
            Assert.Equal(1, SlowCountingMessageHandler.CreatedCount);
        }

    }
}
