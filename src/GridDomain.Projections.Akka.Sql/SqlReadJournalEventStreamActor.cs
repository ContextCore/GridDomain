using System;
using Akka;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams.Dsl;
using GridDomain.EventHandlers;
using GridDomain.EventHandlers.Akka;

namespace GridDomain.Projections.Akka.Sql
{
    public abstract class SqlReadJournalEventStreamActor<TMessage> : EventStreamActor<TMessage> where TMessage : class
    {
        protected override Source<EventEnvelope, NotUsed> GetSource()
        {
            var journal = PersistenceQuery.Get(Context.System).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
            return journal.EventsByTag(nameof(TMessage), GetOffset());
        }

        protected abstract Offset GetOffset();
    }
}