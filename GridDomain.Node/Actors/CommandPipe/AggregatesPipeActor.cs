using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AggregatesPipeActor : ReceiveActor
    {
        private readonly ICatalog<Processor, ICommand> _aggregateCatalog;

        public AggregatesPipeActor(ICatalog<Processor, ICommand> aggregateCatalog)
        {
            _aggregateCatalog = aggregateCatalog;

            Receive<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           var aggregateProcessor = _aggregateCatalog.Get(c.Message);
                                                           if (aggregateProcessor == null)
                                                               throw new CannotFindAggregateForCommandExñeption(c.Message,
                                                                                                                c.Message.GetType());

                                                           aggregateProcessor.ActorRef.Tell(c);
                                                       });
        }
    }
}