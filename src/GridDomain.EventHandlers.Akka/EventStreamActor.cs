using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Node.Akka.Actors;

namespace GridDomain.EventHandlers.Akka
{
    public abstract class EventStreamActor<TEvent, THandler>:ReceiveActor where TEvent : class, IDomainEvent where THandler : IEventHandler<TEvent>
    {
        protected readonly ILoggingAdapter Log;
        private ActorMaterializer _materializer;
        protected abstract Source<EventEnvelope,NotUsed> GetSource();

        protected virtual Flow<EventEnvelope, TEvent, NotUsed> GetFlow()
        {
            return EventsFlow.Create<TEvent>();
        }

        protected virtual Sink<TEvent, NotUsed> GetSink()
        {
            return EventHandlerSink.Create<TEvent, THandler>(Context);
        }
        private BehaviorQueue Behavior { get; }
        
        public EventStreamActor()
        {
            Behavior = new BehaviorQueue(Become);
            Log = Context.GetLogger();
            Behavior.Become(Initializing, nameof(Initializing));
        }
        
        protected virtual void Initializing()
        {
            Receive<EventHandlerActor.Start>(s =>
            {
                var source = GetSource();
                var sink = GetSink();
                var flow = GetFlow();
                
                var projectionGraph = source.Via(flow).To(sink);
                _materializer = Context.System.Materializer();
                projectionGraph.Run(_materializer);

                Behavior.Become(Working, nameof(Working));
            });
        }


        protected virtual void Working()
        {
            
        }
    }
}