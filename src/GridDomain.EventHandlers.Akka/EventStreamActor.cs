using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using GridDomain.Common.Akka;

namespace GridDomain.EventHandlers.Akka
{

    public abstract class EventStreamActor:ReceiveActor 
    {
        
        public class Start
        {
            private Start()
            {
            }

            public static readonly Start Instance = new Start();
        }
        
        public class Started
        {
            private Started()
            {
            }

            public static readonly Started Instance = new Started();
        }
        
        protected readonly ILoggingAdapter Log;
        private ActorMaterializer _materializer;
        protected abstract Source<EventEnvelope,NotUsed> GetSource();

        protected EventStreamActor()
        {
            Behavior = new BehaviorQueue(Become);
            Log = Context.GetLogger();
            Behavior.Become(Initializing, nameof(Initializing));
        }
        protected virtual Flow<EventEnvelope, Sequenced, NotUsed> GetFlow()
        {
            return EventsFlow.Create();
        }

        protected abstract Sink<Sequenced, NotUsed> GetSink();
        
        private BehaviorQueue Behavior { get; }
        
        protected virtual void Initializing()
        {
            Receive<Start>(s =>
            {
                var source = GetSource();
                var sink = GetSink();
                var flow = GetFlow();
                
                var projectionGraph = source.Via(flow).To(sink);
                _materializer = Context.System.Materializer();
                projectionGraph.Run(_materializer);

                Behavior.Become(Working, nameof(Working));
                Sender.Tell(Started.Instance);
            });
        }


        protected virtual void Working()
        {
            
        }
    }
}