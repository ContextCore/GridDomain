using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.Serilog;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AggregatesPipeActor : ReceiveActor
    {
        private ILoggingAdapter Log { get; } = Context.GetLogger(new SerilogLogMessageFormatter());
        public AggregatesPipeActor(ICatalog<IActorRef,object> aggregateCatalog)
        {
            Receive<IMessageMetadataEnvelop>(c =>
                                            {
                                                var aggregateProcessor = aggregateCatalog.Get(c.Message);
                                                if (aggregateProcessor == null)
                                                    throw new CannotFindAggregateForCommandExñeption(c.Message,
                                                                                                     c.Message.GetType());

                                                Log.Debug("Received command {@c}",c);
                                                aggregateProcessor.Tell(c, Sender);
                                            });

        }
    }
}