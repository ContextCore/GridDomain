using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{
    internal class CommandExecutionActor : ReceiveActor
    {
        private readonly AggregateProcessorCatalog _aggregateCatalog;

        public CommandExecutionActor(AggregateProcessorCatalog aggregateCatalog)
        {
            _aggregateCatalog = aggregateCatalog;
        }

        public CommandExecutionActor()
        {
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