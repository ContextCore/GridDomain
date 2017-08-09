using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Hadlers;

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
            ReceiveAsync<IMessageMetadataEnvelop<Project>>(envelop =>
                                                           {
                                                               Log.Debug("Received messages to project. {project}",envelop);
                                                               var project = envelop.Message;
                                                               var envelops = project.Messages.Select(m => CreateMessageMetadataEnvelop(m, envelop.Metadata))
                                                                                     .ToArray();

                                                               var chain = envelops.Select(handlersCatalog.ProcessMessage)
                                                                                   .ToChain();

                                                               return chain.ContinueWith(t =>
                                                                                         {
                                                                                             foreach (var env in envelops)
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
            var @event = message as DomainEvent;
            if (@event != null)
                return new MessageMetadataEnvelop<DomainEvent>(@event, metadata);

            var fault = message as IFault;
            if (fault != null)
                return new MessageMetadataEnvelop<IFault>(fault, metadata);

            return new MessageMetadataEnvelop<object>(message, metadata);
        }
    }
}