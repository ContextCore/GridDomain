using System;
using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.EventHandlers.Akka
{
    public static class EventsFlow
    {
        public static Flow<EventEnvelope, Sequenced<TEvent>, NotUsed> Create<TEvent>() where TEvent : class
        {
            return Flow.Create<EventEnvelope>()
                       .Select(e => new Sequenced<TEvent>(e.Event as TEvent,e.SequenceNr));
        }

        public static Flow<EventEnvelope, Sequenced, NotUsed> Create()
        {
            return Flow.Create<EventEnvelope>()
                       .Select(e => new Sequenced(e.Event,e.SequenceNr));
        }
    }
}