using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Node.Actors.Logging;
using GridDomain.Transport.Extension;

namespace GridDomain.Node.Actors.CommandPipe
{
    /// <summary>
    ///     Synhronize message handlers work for domain events produced by command
    ///     If message process policy is set to synchronized, will process such events one after one
    ///     Will process all other messages in parallel
    /// </summary>
    public class HandlersPipeActor : ReceiveActor
    {
        public const string CustomHandlersProcessActorRegistrationName = "CustomHandlersProcessActor";
        private ILoggingAdapter Log { get; } = Context.GetLogger(new SerilogLogMessageFormatter());
        public HandlersPipeActor(IProcessorListCatalog handlersCatalog, IActorRef processManagerPipeActor)
        {
            var publisher = Context.System.GetTransport();
            ReceiveAsync<IMessageMetadataEnvelop>(envelop =>
                                                           {
                                                                 Log.Debug("Received messages to project. {project}",envelop);
                                                               
                                                                 return handlersCatalog.ProcessMessage(envelop)
                                                                                       .ContinueWith(t =>
                                                                                                     {
                                                                                                         processManagerPipeActor.Tell(envelop);
                                                                                                         publisher.Publish(envelop);
                                                                                                         return AllHandlersCompleted.Instance;
                                                                                        })
                                                                                       .PipeTo(Sender);
                                                           });
        }
    }
}