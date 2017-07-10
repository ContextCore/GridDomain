using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.Processors;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AggregatesPipeActor : ReceiveActor
    {
        public AggregatesPipeActor(ICatalog<IMessageProcessor, ICommand> aggregateCatalog)
        {
            Receive<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           var aggregateProcessor = aggregateCatalog.Get(c.Message);
                                                           if (aggregateProcessor == null)
                                                               throw new CannotFindAggregateForCommandExñeption(c.Message,
                                                                                                                c.Message.GetType());

                                                           aggregateProcessor.ActorRef.Tell(c);
                                                       });
        }
    }
}