using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Serializers;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Transport.Extension;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateActor {
    public abstract class AggregateActorPerf
    {
        private const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
        private Counter _counter;
        private readonly ITestOutputHelper _testOutputHelper;
        private IActorRef _aggregateActor;
        private ICommand[] _commands;
        private Guid _aggregateId;

        protected AggregateActorPerf(ITestOutputHelper output,string actorSystemConfig)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
            _commands = Enumerable.Range(0, 1)
                                  .SelectMany(n => CreateAggregatePlan(1))
                                  .ToArray();

            var sys = ActorSystem.Create("test",actorSystemConfig);
            sys.InitLocalTransportExtension();
            sys.InitDomainEventsSerialization(new EventsAdaptersCatalog());
            var dummy = sys.ActorOf<CustomHandlersActorDummy>();
            _aggregateId = Guid.NewGuid();
            _aggregateActor = sys.ActorOf(Props.Create(
                                                       () => new AggregateActor<Balloon>(new BalloonCommandHandler(), new EachMessageSnapshotsPersistencePolicy(), new AggregateFactory(), dummy)),
                                          AggregateActorName.New<Balloon>(_aggregateId).ToString());
        }

        class CustomHandlersActorDummy : ReceiveActor
        {
            public CustomHandlersActorDummy()
            {
                Receive<IMessageMetadataEnvelop>(m => Sender.Tell(AllHandlersCompleted.Instance));
            }
        }

        private IEnumerable<ICommand> CreateAggregatePlan(int changeAmount)
        {
            var random = new Random();
            yield return new InflateNewBallonCommand(random.Next(), _aggregateId);
            for (var num = 0; num < changeAmount; num++)
                yield return new WriteTitleCommand(random.Next(), _aggregateId);
        }

        [PerfSetup]
        public virtual void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions directly by aggregate actor",
            NumberOfIterations = 3,
            RunMode = RunMode.Iterations,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 100)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void MeasureCommandExecutionWithoutProjectionsInMemory()
        {
            Task.WhenAll(_commands.Select(c => _aggregateActor.Ask<CommandExecuted>(new MessageMetadataEnvelop<ICommand>(c, MessageMetadata.Empty))
                                                              .ContinueWith(t => _counter.Increment())))
                .Wait();
        }

        [PerfCleanup]
        public void Cleanup()
        {
        }
    }
}