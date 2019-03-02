using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence.Journal;
using GridDomain.Aggregates;
using GridDomain.Common;

namespace GridDomain.Node.Akka
{    
    public class HoconActorSystemFactory : IActorSystemFactory
    {
        private readonly Config _hoconConfig;
        private readonly string _name;

        public HoconActorSystemFactory(string name, Config hoconConfig)
        {
            _name = name;
            _hoconConfig = hoconConfig;
        }

        public ActorSystem CreateSystem()
        {
            return ActorSystem.Create(_name, _hoconConfig);
        }
    }

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