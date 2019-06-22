using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;

namespace GridDomain.EventHandlers.Akka.Tests
{
    public class TestEventStreamActor : EventStreamActor<TestMessage>
    {
        private readonly Source<EventEnvelope, NotUsed> _testSource;

        public TestEventStreamActor(Source<EventEnvelope,NotUsed> testSource)
        {
            _testSource = testSource;
        }
        protected override Source<EventEnvelope, NotUsed> GetSource()
        {
            return _testSource;
        }

        protected override Sink<Sequenced<TestMessage>, NotUsed> GetSink()
        {
            return EventHandlerSink.Create<TestMessage, TestEventHandler>(Context);
        }
    }
}