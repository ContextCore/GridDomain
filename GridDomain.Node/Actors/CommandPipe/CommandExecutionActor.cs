using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class CommandExecutionActor : ReceiveActor
    {
        private readonly ICatalog<Processor, ICommand> _aggregateCatalog;

        public CommandExecutionActor(ICatalog<Processor, ICommand> aggregateCatalog)
        {
            _aggregateCatalog = aggregateCatalog;

            Receive<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           var aggregateProcessor = _aggregateCatalog.Get(c.Message);
                                                           if (aggregateProcessor == null)
                                                               throw new CannotFindAggregateForCommandEx�eption(c.Message,
                                                                                                                c.Message.GetType());

                                                           aggregateProcessor.ActorRef.Tell(c);
                                                       });
        }
    }
}