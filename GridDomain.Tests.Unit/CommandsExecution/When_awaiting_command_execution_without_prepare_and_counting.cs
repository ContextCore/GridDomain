using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution {
    public class When_awaiting_command_execution_without_prepare_and_counting : NodeTestKit
    {
        
        public When_awaiting_command_execution_without_prepare_and_counting(ITestOutputHelper output) : this(new NodeTestFixture(output)){}
        protected When_awaiting_command_execution_without_prepare_and_counting(NodeTestFixture output) :
            base(output.Add(new BalloonCountingDomainConfiguration())){ }
        
        
        class BalloonCountingDomainConfiguration : IDomainConfiguration
        {
            public void Register(IDomainBuilder builder)
            {
                builder.RegisterAggregate(new BalloonDependencyFactory());
                builder.RegisterHandler<BalloonCreated, CountingMessageHandler>().AsSync();
                builder.RegisterHandler<BalloonCreated, SlowCountingMessageHandler>(c => new SlowCountingMessageHandler(c.Publisher)).AsFireAndForget();
                builder.RegisterHandler<BalloonTitleChanged, CountingMessageHandler>().AsSync();
            }
        }

        class IntResult
        {
            public int Value;
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
            public SlowCountingMessageHandler(IPublisher publisher)
            {
                _publisher = publisher;
            }
            public static int CreatedCount;
            private readonly IPublisher _publisher;

            public async Task Handle(BalloonCreated message, IMessageMetadata metadata = null)
            {
                await Task.Delay(2000);
                _publisher.Publish(new IntResult{Value = 500}, MessageMetadata.Empty);
                CreatedCount++;
            }
        }

      

        [Fact]
        public async Task Then_command_executed_sync_and_parralel_message_processor_are_executed()
        {
            var aggregateId = Guid.NewGuid().ToString();
            var slowCounterWaiter = Node.NewWaiter()
                                        .Expect<IntResult>()
                                        .Create();

            //will produce one created message and two title changed
            await Node.Execute(new InflateCopyCommand(100, aggregateId),
                               new WriteTitleCommand(200, aggregateId));

            Assert.Equal(1, CountingMessageHandler.CreatedCount);
            Assert.Equal(2, CountingMessageHandler.ChangedCount);
            //will not wait until Fire and Forget handlers
            Assert.Equal(0, SlowCountingMessageHandler.CreatedCount);
            await slowCounterWaiter;
            //but Fire and Forget handler was launched and will complete later
            Assert.Equal(1, SlowCountingMessageHandler.CreatedCount);
        }

    }
}