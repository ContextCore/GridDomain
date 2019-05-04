using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors;
using GridDomain.Query;
using Xunit;

namespace GridDomain.Node.Tests.TestJournals
{
    public class DomainEventsByIdQuery:IQuery<string,IDomainEvent>
    {
        private readonly ActorSystem _system;
        private readonly ICurrentEventsByPersistenceIdQuery _streamsQuery;

        public DomainEventsByIdQuery(ActorSystem system, ICurrentEventsByPersistenceIdQuery streamsQuery)
        {
            _streamsQuery = streamsQuery;
            _system = system;
        }
        public async Task<IReadOnlyCollection<IDomainEvent>> Execute(string aggregateId)
        {
            var source = _streamsQuery.CurrentEventsByPersistenceId(aggregateId,0,long.MaxValue)
                                      .Select(e => (IDomainEvent)e.Event);
            var sink = Sink.Seq<IDomainEvent>();
            // connect the Source to the Sink, obtaining a RunnableGraph
            var runnable = source.ToMaterialized(sink, Keep.Right);
            
            using (var materializer = _system.Materializer())
            {
                return await runnable.Run(materializer);
            }
        }
    }
}