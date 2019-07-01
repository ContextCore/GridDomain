using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Cluster;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Abstractions;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Common.Akka;
using GridDomain.Domains;
using GridDomain.Node.Akka.Extensions.GridDomain;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.EventHandlers.Akka.Tests
{
    public class EventHandlerRegistrationTests : TestKit
    {
        private static Config akkaConfig = @"
akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
akka.cluster.roles=[api, projection, calculation]
akka.loggers = [""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]
akka.persistence{
	journal {
        plugin = ""akka.persistence.journal.inmem""
		}
	}

	snapshot-store {
		plugin = ""akka.persistence.snapshot-store.inmem""
		}
	}
}
";

        public EventHandlerRegistrationTests(ITestOutputHelper output) : base(akkaConfig.WithFallback(TestKitBase.FullDebugConfig), "test")
        {
            Cluster.Get(Sys).Join(Sys.GetAddress());
            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.TestOutput(output).MinimumLevel.Is(LogEventLevel.Verbose).CreateLogger();
        }


        [Fact]
        public async Task Given_registered_event_handler_When_pushing_messages_Then_handler_receives_it()
        {
            var eventEnvelopes = new[]
            {
                new EventEnvelope(Offset.Sequence(1), "test", 1, new TestMessage(10)),
                new EventEnvelope(Offset.Sequence(2), "test", 2, new TestMessage(20)),
                new EventEnvelope(Offset.Sequence(3), "test", 3, new TestMessage(30)),
            };
            var source = Source.From(eventEnvelopes);

            var testEventHandler = new TestEventHandler();
            var messageStore = testEventHandler.Messages;
            var container = new ContainerBuilder();
            container.RegisterInstance(source)
                .Named<Source<EventEnvelope, NotUsed>>(typeof(TestEventHandler).Get<TestMessage>());
            container.RegisterInstance(testEventHandler);


            var ext = Sys.InitEventHandlersExtension(container);
            ext.RegisterEventHandler<TestMessage, TestEventHandler>();
            await ext.Build();

            await Task.Delay(500);

            Assert.Equal(eventEnvelopes.Select(s => s.Event as TestMessage).ToArray(), messageStore);
        }


        [Fact]
        public async Task Given_node_registered_event_handler_When_pushing_messages_Then_handler_receives_it()
        {
            var eventEnvelopes = new[]
            {
                new EventEnvelope(Offset.Sequence(1), "test", 1, new TestMessage(10)),
                new EventEnvelope(Offset.Sequence(2), "test", 2, new TestMessage(20)),
                new EventEnvelope(Offset.Sequence(3), "test", 3, new TestMessage(30)),
            };
            var source = Source.From(eventEnvelopes);

            var testEventHandler = new TestEventHandler();
            var messageStore = testEventHandler.Messages;
            var container = new ContainerBuilder();
            container.RegisterInstance(source)
                .Named<Source<EventEnvelope, NotUsed>>(typeof(TestEventHandler).Get<TestMessage>());
            container.RegisterInstance(testEventHandler);

            var nodeBuilder = Sys.InitGridDomainExtension();
            nodeBuilder.Register<IEventHandlersDomainBuilder>(s => s.InitEventHandlersExtension(container));
            nodeBuilder.Add(new TestEventHandlerConfiguration());
            var node = nodeBuilder.Build();
            await node.Start();

            await Task.Delay(500);

            Assert.Equal(eventEnvelopes.Select(s => s.Event as TestMessage).ToArray(), messageStore);
        }


        [Fact]
        public async Task
            Given_node_registered_event_handler_for_two_messages_When_pushing_messages_Then_handler_receives_it()
        {
            var eventEnvelopes = new[]
            {
                new EventEnvelope(Offset.Sequence(1), "test", 1, new TestMessage(10)),
                new EventEnvelope(Offset.Sequence(2), "test", 2, new TestMessage(20)),
                new EventEnvelope(Offset.Sequence(1), "test", 1, new AnotherTestMessage(30)),
            };
            var source = Source.From(eventEnvelopes);

            var testEventHandler = new TestEventHandler();
            var container = new ContainerBuilder();

            container.RegisterInstance(source)
                .Named<Source<EventEnvelope, NotUsed>>("testSource");
            container.RegisterInstance(testEventHandler);

            await Sys.InitGridDomainExtension()
                    .Register<IEventHandlersDomainBuilder>(s => s.InitEventHandlersExtension(container))
                    .Add(new TestEventHandlerWithTwoSubscriptionsConfiguration("testSource"))
                    .Build()
                    .Start();

            await Task.Delay(1000);

            var receivedMessages = testEventHandler.Messages.Concat<object>(testEventHandler.AnotherMessages);
            Assert.Equal(eventEnvelopes.Select(s => s.Event).ToArray(), receivedMessages.ToArray());
        }

        public class TestEventHandlerConfiguration : DomainConfiguration<IEventHandlersDomainBuilder>
        {
            protected override Task ConfigurePart(IEventHandlersDomainBuilder partBuilder)
            {
                partBuilder.RegisterEventHandler<TestMessage, TestEventHandler>();
                return Task.CompletedTask;
            }
        }

        public class
            TestEventHandlerWithTwoSubscriptionsConfiguration : DomainConfiguration<IEventHandlersDomainBuilder>
        {
            private readonly string _sourceName;

            public TestEventHandlerWithTwoSubscriptionsConfiguration(string sourceName)
            {
                _sourceName = sourceName;
            }
            protected override Task ConfigurePart(IEventHandlersDomainBuilder partBuilder)
            {
                partBuilder.RegisterEventHandler<TestMessage, AnotherTestMessage, TestEventHandler>(_sourceName);
                return Task.CompletedTask;
            }
        }
    }
}