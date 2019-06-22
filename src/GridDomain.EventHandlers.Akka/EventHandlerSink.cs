using Akka;
using Akka.Actor;
using Akka.Streams.Dsl;

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
    }
}