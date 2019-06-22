using System;
using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using GridDomain.Common;

namespace GridDomain.EventHandlers.Akka
{

    public static class StreamName
    {
        public static string GetStreamName<TMessage>(this Type handlerType)
        {
            return $"{handlerType.BeautyName()}_{typeof(TMessage).BeautyName()}";
        }
    }
    public class SingleHandlerEventStreamActor<TEvent, THandler> : EventStreamActor<TEvent> where TEvent : class where THandler : IEventHandler<TEvent>
    {
        private readonly string _eventSourceName;

        public SingleHandlerEventStreamActor(string eventSourceName)
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
}