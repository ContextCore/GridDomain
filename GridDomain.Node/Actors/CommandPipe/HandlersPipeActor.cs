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
            ReceiveAsync<IMessageMetadataEnvelop<Project>>(envelop =>
                                                           {
                                                               Log.Debug("Received messages to project. {project}",envelop);
                                                               var project = envelop.Message;
                                                               var envelops = project.Messages.Select(m => CreateMessageMetadataEnvelop(m, envelop.Metadata))
                                                                                     .ToArray();

                                                               var chain = envelops.Select(e => {
                                                                                             //TODO: replace with direct metadata publish
                                                                                             return handlersCatalog.ProcessMessage(e)
                                                                                                                   .ContinueWith(t =>
                                                                                                                                 {
                                                                                                                                        publisher.Publish(e.Message, envelop.Metadata);
                                                                                                                                 });
                                                                                               
                                                                                           }).ToChain();

                                                               return chain.ContinueWith(t =>
                                                                                         {

                                                                                             foreach(var env in envelops)
                                                                                                 processManagerPipeActor.Tell(env);
                                                                                             var completed = new AllHandlersCompleted(project.ProjectId);
                                                                                             Log.Debug("Pack projected. {project}", completed);
                                                                                             return completed;

                                                                                         })
                                                                           .PipeTo(Sender);
                                                           });
        }

        private static IMessageMetadataEnvelop CreateMessageMetadataEnvelop(object message, IMessageMetadata metadata)
        {
            if(message is DomainEvent @event)
                return new MessageMetadataEnvelop<DomainEvent>(@event, metadata);

            if(message is IFault fault)
                return new MessageMetadataEnvelop<IFault>(fault, metadata);

            return new MessageMetadataEnvelop<object>(message, metadata);
        }
    }
}