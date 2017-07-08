using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;
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

        public HandlersPipeActor(IProcessorListCatalog handlersCatalog, IActorRef sagasProcessActor)
        {
            ReceiveAsync<IMessageMetadataEnvelop<Project>>(envelop =>
                                                           {
                                                               var sender = Sender;
                                                               var project = envelop.Message;
                                                               var envelops = project.Messages.Select(m => CreateMessageMetadataEnvelop(m,envelop.Metadata)).ToArray();

                                                               return envelops.Select(messageMetadataEnvelop => SynhronizeHandlers(messageMetadataEnvelop, handlersCatalog))
                                                                              .ToChain()
                                                                              .ContinueWith(t =>
                                                                                            {
                                                                                                sender.Tell(new AllHandlersCompleted(project.ProjectId));

                                                                                                foreach (var env in envelops)
                                                                                                    sagasProcessActor.Tell(env);
                                                                                            });
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

        private Task SynhronizeHandlers(IMessageMetadataEnvelop messageMetadataEnvelop, IProcessorListCatalog processorListCatalog)
        {
            var processors = processorListCatalog.Get(messageMetadataEnvelop.Message);
            if (!processors.Any())
                return Task.CompletedTask;

            return processors.Select(p =>
                                          {
                                              if (p.Policy.IsSynchronious)
                                                  return p.ActorRef.Ask<HandlerExecuted>(messageMetadataEnvelop);

                                              p.ActorRef.Tell(messageMetadataEnvelop);
                                              return Task.CompletedTask;
                                          })
                              .ToChain();
        }
    }
}