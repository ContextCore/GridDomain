using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class Project
    {
        public Project(object[] messages, Guid projectId)
        {
            Messages = messages;
            ProjectId = projectId;
        }

        public Project(params object[] messages) : this(messages, Guid.NewGuid()) {}

        public object[] Messages { get; }

        public Guid ProjectId { get; }
    }

    /// <summary>
    ///     Synhronize message handlers work for domain events produced by command
    ///     If message process policy is set to synchronized, will process such events one after one
    ///     Will process all other messages in parallel
    /// </summary>
    public class HandlersProcessActor : ReceiveActor
    {
        public const string CustomHandlersProcessActorRegistrationName = "CustomHandlersProcessActor";

        private readonly IProcessorListCatalog _handlersCatalog;

        public HandlersProcessActor(IProcessorListCatalog handlersCatalog, IActorRef sagasProcessActor)
        {
            _handlersCatalog = handlersCatalog;

            ReceiveAsync<IMessageMetadataEnvelop<Project>>(envelop =>
                                                           {
                                                               var sender = Sender;
                                                               var project = envelop.Message;
                                                               var metadata = envelop.Metadata;
                                                               var envelops = project.Messages.Select(e => new MessageMetadataEnvelop<object>(e, metadata)).ToArray();

                                                               return envelops.Select(SynhronizeHandlers)
                                                                              .ToChain()
                                                                              .ContinueWith(t =>
                                                                                            {
                                                                                                sender.Tell(new AllHandlersCompleted(project.ProjectId));

                                                                                                foreach (var env in envelops)
                                                                                                    sagasProcessActor.Tell(env);
                                                                                            });
                                                           });
        }

        private Task SynhronizeHandlers(IMessageMetadataEnvelop messageMetadataEnvelop)
        {
            var faultProcessors = _handlersCatalog.Get(messageMetadataEnvelop.Message);
            if (!faultProcessors.Any())
                return Task.CompletedTask;

            return faultProcessors.Select(p =>
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