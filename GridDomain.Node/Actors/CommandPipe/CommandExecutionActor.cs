using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;

namespace GridDomain.Node.Actors.CommandPipe
{
    internal class CommandExecutionActor : ReceiveActor
    {
        private readonly IAggregateProcessorCatalog _aggregateCatalog;

        public CommandExecutionActor(IAggregateProcessorCatalog aggregateCatalog)
        {
            _aggregateCatalog = aggregateCatalog;

            Receive<IMessageMetadataEnvelop<ICommand>>(c =>
            {
                Processor aggregateProcessor = _aggregateCatalog.GetAggregateProcessor(c.Message);
                if (aggregateProcessor == null)
                    throw new CannotFindAggregateForCommandExeption(c.Message, c.Message.GetType());

                aggregateProcessor.ActorRef.Tell(c);
            });
        }
    }
}