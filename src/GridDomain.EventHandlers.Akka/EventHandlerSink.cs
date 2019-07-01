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
        public static Sink<Sequenced<TEvent>, NotUsed> Create<TEvent, THandler>(IActorRefFactory system)
            where THandler : IEventHandler<TEvent>
        {
            var actorName = $"{typeof(THandler).Name}_{typeof(TEvent).Name}";

            var actorRef = system.ActorOf(Props.Create<EventHandlerActor<TEvent, THandler>>(), actorName);

            return Sink.ActorRefWithAck<Sequenced<TEvent>>(
                actorRef,
                EventHandlerActor.Start.Instance,
                EventHandlerActor.Next.Instance,
                EventHandlerActor.Done.Instance);

        }

        public static Sink<Sequenced, NotUsed> Create<TEventA,TEventB,THandler>(IActorRefFactory system,
            string name = null) where THandler : IEventHandler<TEventA>, IEventHandler<TEventB>
        {
            var actorName = name ?? String.Join("_",new []{typeof(THandler),typeof(TEventA),typeof(TEventB)}.Select(t => t.BeautyName()));

            var actorRef = system.ActorOf(Props.Create<EventHandlerActor<TEventA,TEventB,THandler>>(), actorName);

            return Sink.ActorRefWithAck<Sequenced>(
                actorRef,
                EventHandlerActor.Start.Instance,
                EventHandlerActor.Next.Instance,
                EventHandlerActor.Done.Instance);

        }
    }
}