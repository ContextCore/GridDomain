using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AggregatesPipeActor : ReceiveActor
    {
        private ILoggingAdapter Log { get; } = Context.GetLogger(new SerilogLogMessageFormatter());
        public AggregatesPipeActor(ICatalog<IMessageProcessor, ICommand> aggregateCatalog)
        {
            ReceiveAsync<IMessageMetadataEnvelop<ICommand>>(c =>
                                                       {
                                                           var aggregateProcessor = aggregateCatalog.Get(c.Message);
                                                           if (aggregateProcessor == null)
                                                               throw new CannotFindAggregateForCommandExñeption(c.Message,
                                                                                                                c.Message.GetType());

                                                           var workInProgressTask = Task.CompletedTask;
                                                           Log.Debug("Received command {@c}",c);
                                                           aggregateProcessor.Process(c, ref workInProgressTask);
                                                           return workInProgressTask;
                                                       });
        }
    }
}