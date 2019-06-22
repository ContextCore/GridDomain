using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Akka.TestKit.Xunit2;
using Autofac;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.EventHandlers.Akka.Tests
{
    public class EventHandlerTests:TestKit
    {
        public EventHandlerTests(ITestOutputHelper output):base("",output)
        {
            
        }
        [Fact]
        public async Task Given_stream_and_EventHandler_When_pushing_messages_Then_handler_receives_it()
        {
            var eventEnvelopes = new[]
            {
                new EventEnvelope(Offset.Sequence(1),"test",1,new TestMessage(10)),
                new EventEnvelope(Offset.Sequence(2),"test",2,new TestMessage(20)),
                new EventEnvelope(Offset.Sequence(3),"test",3,new TestMessage(30)),
                    
            };
            var source = Source.From(eventEnvelopes);


            var testEventHandler = new TestEventHandler();
            var messageStore = testEventHandler.Messages;
            
            var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterInstance(testEventHandler);
                
            Sys.InitEventHandlersExtension(containerBuilder.Build());
            var streamActor = Sys.ActorOf(Props.Create(() => new TestEventStreamActor(source)), "StreamActor");
            
            streamActor.Tell(EventStreamActor.Start.Instance);
            ExpectMsg<EventStreamActor.Started>();

            await Task.Delay(500);
            
           Assert.Equal(eventEnvelopes.Select(s => s.Event as TestMessage).ToArray(),messageStore);

        }
    }
}