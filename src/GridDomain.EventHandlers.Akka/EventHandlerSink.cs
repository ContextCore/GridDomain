using System;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Streams.Dsl;
using GridDomain.Common;

namespace GridDomain.EventHandlers.Akka
{
    public static class EventHandlerSink
    {
        public static Sink<Sequenced, NotUsed> Create<TEvent, THandler>(IActorRefFactory system, string name=null)
            where THandler : IEventHandler<TEvent>
        {
            var actorName = name ?? "EventHandler_"+SourceName.Get<THandler,TEvent>();
            return Create(system, Props.Create<EventHandlerActor<TEvent, THandler>>(), actorName);

        }

        private static Sink<Sequenced, NotUsed> Create(IActorRefFactory system, Props props, string name)
        {
            var actorRef = system.ActorOf(props, name);

            return Sink.ActorRefWithAck<Sequenced>(
                actorRef,
                EventHandlerActor.Start.Instance,
                EventHandlerActor.Next.Instance,
                EventHandlerActor.Done.Instance);
        }
        
        public static Sink<Sequenced, NotUsed> Create<TEventA,TEventB,THandler>(IActorRefFactory system,
            string name = null) where THandler : IEventHandler<TEventA>, IEventHandler<TEventB>
        {
            var actorName = name ?? "EventHandler_"+SourceName.Get<THandler,TEventA,TEventB>();
            return Create(system, Props.Create<EventHandlerActor<TEventA, TEventB, THandler>>(), actorName);
        }
    }
}