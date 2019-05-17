using Akka.Persistence.Journal;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Node.Akka
{
    public class AggregateTaggingAdapter : IEventAdapter
    {
        public string Manifest(object evt)
        {
            return evt.GetType().FullName;
        }

        public object ToJournal(object evt)
        {
            return new Tagged(evt,new []{((IDomainEvent)evt).Source.Name, "aggregate"});
        }

        public IEventSequence FromJournal(object evt, string manifest)
        {
            return EventSequence.Single(evt);
        }
    }
}