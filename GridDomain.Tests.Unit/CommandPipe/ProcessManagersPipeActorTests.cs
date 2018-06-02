using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandPipe
{
    public class ProcessManagersPipeActorTests : TestKit
    {
        private readonly ITestOutputHelper _output;

        public ProcessManagersPipeActorTests(ITestOutputHelper output)
        {
            _output = output;
            Sys.AttachSerilogLogging(new XUnitAutoTestLoggerConfiguration(output).CreateLogger());
        }
        [Fact]
        public async Task All_Processes_performs_linear_and_results_from_all_processes_are_gathered()
        {
            var processAId = Guid.NewGuid().ToString();
            _output.WriteLine("Process A:" + processAId);
            var testProcessActorA =
                Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, processAId, TimeSpan.FromMilliseconds(1000))));

            var processBId = Guid.NewGuid().ToString();
            _output.WriteLine("Process B:" + processBId);

            var testProcessActorB =
                Sys.ActorOf(Props.Create(() =>new TestProcessActor(TestActor,
                                                                   processBId,
                                                                   TimeSpan.FromMilliseconds(50))));
            var processCId = Guid.NewGuid().ToString();
            _output.WriteLine("Process C:" + processCId);

            var testProcessActorC =
                Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, processCId, TimeSpan.FromMilliseconds(50))));

            var catalog = new ProcessesDefaultProcessor();

            catalog.Add<BalloonCreated>(new SyncProcessManagerProcessor(testProcessActorA));
            catalog.Add<BalloonTitleChanged>(new SyncProcessManagerProcessor(testProcessActorB));
            catalog.Add<BalloonTitleChanged>(new SyncProcessManagerProcessor(testProcessActorC));


            var balloonCreated = new BalloonCreated("1", Guid.NewGuid().ToString());
            var balloonTitleChanged = new BalloonTitleChanged("2", Guid.NewGuid().ToString());


            //var resultA = await catalog.Process(MessageMetadataEnvelop.New<DomainEvent>(balloonCreated));
            //var resultB = await catalog.Process(MessageMetadataEnvelop.New<DomainEvent>(balloonTitleChanged));







            var processPipeActor = Sys.ActorOf(Props.Create(() => new LocalProcessesPipeActor(catalog)));
            await processPipeActor.Ask<Initialized>(new Initialize(TestActor));

            processPipeActor.Tell(MessageMetadataEnvelop.New<DomainEvent>(balloonCreated));
            processPipeActor.Tell(MessageMetadataEnvelop.New<DomainEvent>(balloonTitleChanged));

            //process pipe will process domain event linear on each message
            //but for don't wait for each message execution end, so first will complete process of second message - balloonTitleChanged

            //after process pipe will proceed with balloonTitleChanged event, pass it linear to two left process managers

            var transited = ExpectMsg<ProcessTransited>(TimeSpan.FromSeconds(600));
            var testCommand = transited.ProducedCommands.OfType<TestCommand>().First();
            Assert.Equal(processBId, testCommand.ProcessId);


            transited = ExpectMsg<ProcessTransited>();
            testCommand = transited.ProducedCommands.OfType<TestCommand>().First();
            Assert.Equal(processCId, testCommand.ProcessId);
            
            //after it process pipe is finished with ballon created message processing, gathering results
            //and sending it to commandPipe (testActor)
            var cmdB = ExpectMsg<MessageMetadataEnvelop<ICommand>>();
            Assert.Equal(processBId, cmdB.Message.ProcessId);

            var cmdC = ExpectMsg<MessageMetadataEnvelop<ICommand>>();
            Assert.Equal(processCId, cmdC.Message.ProcessId);

            //than it will report end of domain event processing
            ExpectMsg<ProcessesTransitComplete>();

            //than slow processing of first message will finish 

            //test process actors sends to us messages on complete
            //wait for test process actor transit on first event

            transited = ExpectMsg<ProcessTransited>();
            testCommand = transited.ProducedCommands.OfType<TestCommand>().First();
            Assert.Equal(processAId, testCommand.ProcessId);
          
            var cmdA = ExpectMsg<MessageMetadataEnvelop<ICommand>>();
            Assert.Equal(processAId, cmdA.Message.ProcessId);
            //process pipe has only one handler for balloonCreated, so it will finish processing
            //and send us a message 
            ExpectMsg<ProcessesTransitComplete>();
        }

        class Inherited : BalloonCreated
        {
            public Inherited() : base("123", Guid.NewGuid().ToString()) { }
        }

        [Fact]
        public async Task ProcessManagerPipeActor_does_not_support_domain_event_inheritance()
        {
            var testProcessActor = Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, Guid.NewGuid().ToString(),null)));
            var catalog = new ProcessesDefaultProcessor();
            catalog.Add<BalloonCreated>(new SyncProcessManagerProcessor(testProcessActor));

            var processPipeActor = Sys.ActorOf(Props.Create(() => new LocalProcessesPipeActor(catalog)));
            await processPipeActor.Ask<Initialized>(new Initialize(TestActor));

            var msg = MessageMetadataEnvelop.New<DomainEvent>(new Inherited());

            processPipeActor.Tell(msg);
            ExpectMsg<ProcessesTransitComplete>();
            //process processor did not run due to error, but we received processing complete message
            ExpectNoMsg(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ProcessPipeActor_routes_events_by_type()
        {
            var testProcessActor = Sys.ActorOf(Props.Create(() => new TestProcessActor(TestActor, Guid.NewGuid().ToString(), null)));

            var catalog = new ProcessesDefaultProcessor();
            catalog.Add<BalloonCreated>(new SyncProcessManagerProcessor(testProcessActor));

            var processPipeActor = Sys.ActorOf(Props.Create(() => new LocalProcessesPipeActor(catalog)));
            await processPipeActor.Ask<Initialized>(new Initialize(TestActor));


            var msg = new MessageMetadataEnvelop<DomainEvent>(new BalloonCreated("1", Guid.NewGuid().ToString()),
                                                              MessageMetadata.Empty);

            processPipeActor.Tell(msg);

            //TestActor from test process processor receives message after work is done
            ExpectMsg<ProcessTransited>();
            //process pipe should send next step - command execution actor that new commands should be executed
            ExpectMsg<IMessageMetadataEnvelop<ICommand>>();
        }
    }
}