using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;

namespace GridDomain.EventHandlers.Akka
{
    public class HandlerEventStreamActor<TEvent, THandler> : EventStreamActor<TEvent> where TEvent : class where THandler : IEventHandler<TEvent>
    {
        private readonly string _eventSourceName;

        public HandlerEventStreamActor(string eventSourceName)
        {
            _eventSourceName = eventSourceName;
        }
        protected override Source<EventEnvelope, NotUsed> GetSource()
        {
            return Context.System.GetEventHandlersExtension().GetSource(_eventSourceName);
        }

        protected override Sink<Sequenced<TEvent>, NotUsed> GetSink()
        {
            return EventHandlerSink.Create<TEvent, THandler>(Context);
        }
    }
    
    public class HandlerEventStreamActor<TEventA, TEventB, THandler> : MultiEventStreamActor where TEventA : class where TEventB:class
        where THandler : IEventHandler<TEventA>, IEventHandler<TEventB>
    {
        private readonly string _eventSourceName;

        public HandlerEventStreamActor(string eventSourceName)
        {
            _eventSourceName = eventSourceName;
        }
        protected override Source<EventEnvelope, NotUsed> GetSource()
        {
            Log.Debug("Getting source {name}",_eventSourceName);
            return Context.System.GetEventHandlersExtension().GetSource(_eventSourceName);
        }

        protected override Sink<Sequenced, NotUsed> GetSink()
        {
            return EventHandlerSink.Create<TEventA,TEventB, THandler>(Context);
        }
    }
}