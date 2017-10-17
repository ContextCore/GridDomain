using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.TestActors;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Serializers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Transport.Extension;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.Processes
{
   //public class ProcessActorPerf
   //{
   //    private const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
   //    private Counter _counter;
   //    private readonly IActorRef _processActor;
   //    private readonly DomainEvent[] _events;
   //    private readonly Guid _aggregateId;
   //
   //    protected ProcessActorPerf(ITestOutputHelper output, string actorSystemConfig)
   //    {
   //        Trace.Listeners.Clear();
   //        Trace.Listeners.Add(new XunitTraceListener(output));
   //        _events = Enumerable.Range(0, 40)
   //                              .SelectMany(n => CreateProcessPlan(40))
   //                              .ToArray();
   //
   //        var sys = ActorSystem.Create("test", actorSystemConfig);
   //        sys.InitLocalTransportExtension();
   //        sys.InitDomainEventsSerialization(new EventsAdaptersCatalog());
   //        var dummy = sys.ActorOf<CustomHandlersActorDummy>();
   //        _aggregateId = Guid.NewGuid();
   //
   //        var handlersActor = sys.ActorOf(Props.Create(() => new CustomHandlersActorDummy()));
   //
   //        var processStateActor = sys.ActorOf(Props.Create(() => new ProcessStateActor<SoftwareProgrammingState>(CommandAggregateHandler.New<ProcessStateAggregate<SoftwareProgrammingState>>(),
   //                                                                                                               new EachMessageSnapshotsPersistencePolicy(),
   //                                                                                                               new AggregateFactory(),
   //                                                                                                               handlersActor)),
   //                                                                                                               "SoftwareProgrammingState_Hub");
   //        _processActor = sys.ActorOf(Props.Create(() => new ProcessActor<SoftwareProgrammingState>(new SoftwareProgrammingProcess(),
   //                                                                                                  new SoftwareProgrammingProcessStateFactory())),
   //                                      EntityActorName.New<Balloon>(_aggregateId).ToString());
   //    }
   //
   //    class CustomHandlersActorDummy : ReceiveActor
   //    {
   //        public CustomHandlersActorDummy()
   //        {
   //            Receive<IMessageMetadataEnvelop>(m => Sender.Tell(AllHandlersCompleted.Instance));
   //        }
   //    }
   //
   //    private IEnumerable<DomainEvent> CreateProcessPlan(int changeAmount)
   //    {
   //        var random = new Random();
   //        yield return new InflateNewBallonCommand(random.Next(), _aggregateId);
   //        for(var num = 0; num < changeAmount; num++)
   //            yield return new WriteTitleCommand(random.Next(), _aggregateId);
   //    }
   //
   //    [PerfSetup]
   //    public virtual void Setup(BenchmarkContext context)
   //    {
   //        _counter = context.GetCounter(TotalCommandsExecutedCounter);
   //    }
   //
   //    [NBenchFact]
   //    [PerfBenchmark(Description = "Measuring command executions directly by aggregate actor",
   //        NumberOfIterations = 3,
   //        RunMode = RunMode.Iterations,
   //        TestMode = TestMode.Test)]
   //    [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 100)]
   //    [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
   //    public void MeasureCommandExecution()
   //    {
   //        Task.WhenAll(_events.Select(c => _processActor.Ask<CommandExecuted>(new MessageMetadataEnvelop<ICommand>(c, MessageMetadata.Empty))
   //                                                          .ContinueWith(t => _counter.Increment())))
   //            .Wait();
   //    }
   //
   //    [PerfCleanup]
   //    public void Cleanup()
   //    {
   //    }
   //}
}
