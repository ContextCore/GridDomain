using System;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams.Dsl;
using GridDomain.EventHandlers;
using GridDomain.EventHandlers.Akka;

namespace GridDomain.Projections.Akka.Sql
{
    public static class SqlSourceExtensions
    {
        public static Source<EventEnvelope, NotUsed> GetSource<TMessage>(this ActorSystem system, Offset offset=null)
        {
            var journal = PersistenceQuery.Get(system).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
            return journal.EventsByTag(nameof(TMessage), offset??Offset.NoOffset()); 
        }
        
    }
}