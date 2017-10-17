using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Actors.ProcessManagers.Exceptions;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Node.AkkaMessaging;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;
using GridDomain.Transport.Extension;

namespace GridDomain.Node.Actors.ProcessManagers
{
    //TODO: add status info, e.g. was any errors during execution or recover
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class ProcessActor<TState> : ReceiveActor,
                                        IWithUnboundedStash where TState : class, IProcessState
    {
        private readonly ProcessEntry _exceptionOnTransit;
        private readonly ProcessEntry _producedCommand;
        private readonly IProcessStateFactory<TState> _processStateFactory;
        private readonly IPublisher _publisher;
        private readonly ILoggingAdapter _log;
        private BehaviorQueue Behavior { get; }
        private ActorMonitor Monitor { get; }
        private IActorRef _stateAggregateActor;
        private static readonly string ProcessStateActorSelection = "user/" + typeof(TState).BeautyName() + "_Hub";
        private readonly ActorSelection _stateActorSelection;
        public IProcess<TState> Process { get; private set; }
        public TState State { get; private set; }

        public IStash Stash { get; set; }

        private Guid Id { get; }
        public ProcessActor(IProcess<TState> process, IProcessStateFactory<TState> processStateFactory)

        {
            Process = process;
            Monitor = new ActorMonitor(Context, "Process" + typeof(TState).Name);
            Behavior = new BehaviorQueue(Become);

            if (!EntityActorName.TryParseId(Self.Path.Name, out var id))
                throw new BadNameFormatException();
            Id = id;

            _publisher = Context.System.GetTransport();
            _processStateFactory = processStateFactory;
            _log = Context.GetSeriLogger();

            _exceptionOnTransit = ProcessManagerActorConstants.ExceptionOnTransit(Self.Path.Name);
            _producedCommand = ProcessManagerActorConstants.ProcessProduceCommands(Self.Path.Name);
            _stateActorSelection = Context.System.ActorSelection(ProcessStateActorSelection);

            Behavior.Become(InitializingBehavior, nameof(InitializingBehavior));
        }

        private void StashingMessagesToProcessBehavior(string reason)
        {
            Receive<IMessageMetadataEnvelop>(m => StashMessage(m, reason));
            Receive<ProcessRedirect>(m => StashMessage(m, reason));
            ProxifyingCommandsBehavior();
        }

        private void InitializingBehavior()
        {
            _stateActorSelection.ResolveOne(TimeSpan.FromSeconds(10))
                                .PipeTo(Self);

            Receive<Status.Failure>(f => throw new CannotFindProcessStatePersistenceActor(ProcessStateActorSelection));
            Receive<IActorRef>(r =>
                              {
                                  _stateAggregateActor = r;
                                  _stateAggregateActor.Tell(new GetProcessState(Id));
                              });
            Receive<ProcesStateMessage<TState>>(ss =>
                                                {
                                                    State = ss.State;
                                                    Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                                                });
            StashingMessagesToProcessBehavior("process is initializing");
        }

        private void StashMessage(object m, string reason)
        {
            _log.Debug("Stashing message {message} because {reason}", m, reason);
            Stash.Stash();
        }

        private void AwaitingMessageBehavior()
        {
            Stash.Unstash();

            Receive<IMessageMetadataEnvelop>(env =>
                                             {

                                                 if (env.Message is ProcessRedirect redirect)
                                                 {
                                                     _log.Debug("Received redirected message");
                                                     Self.Tell(redirect.MessageToRedirect, Sender);
                                                     Behavior.Become(TransitingProcessBehavior, nameof(TransitingProcessBehavior));
                                                     return;
                                                 }

                                                 if (State == null)
                                                 {
                                                     Self.Tell(new CreateNewProcess(env), Sender);
                                                     Behavior.Become(CreatingProcessBehavior, nameof(CreatingProcessBehavior));
                                                 }
                                                 else
                                                 {
                                                     if(Id != GetProcessId(env.Message))
                                                     {
                                                         _log.Error("Received message {@message} "
                                                                    + "targeting different process. Process will not proceed.",
                                                                    env);

                                                         FinishWithError(env, Sender, new ProcessIdMismatchException());
                                                         return;
                                                     }

                                                     Self.Tell(env, Sender);
                                                     Behavior.Become(TransitingProcessBehavior, nameof(TransitingProcessBehavior));
                                                 }
                                             });
            ProxifyingCommandsBehavior();
        }

        private void ProxifyingCommandsBehavior()
        {
            Receive<GracefullShutdownRequest>(r =>
                                              {
                                                  var remangingMessages = Stash.ClearStash()
                                                                               .ToArray();
                                                 
                                                  if(!remangingMessages.Any())
                                                  {
                                                      _log.Debug("Terminating process");
                                                      Context.Stop(Self);
                                                  }
                                                  else
                                                  {
                                                      _log.Debug("Process termination is requested, but got {count} messages to process, will process them first and terminate after", remangingMessages.Length);
                                                      Stash.Stash();

                                                      foreach(var message in remangingMessages)
                                                          Self.Tell(message);
                                                  }
                                              });


            Receive<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Receive<NotifyOnPersistenceEvents>(c => ((ICanTell)_stateAggregateActor ?? _stateActorSelection).Tell(c, Sender));
        }

        private void CreatingProcessBehavior()
        {
            TState pendingState = null;
            IMessageMetadataEnvelop processingMessage = null;
            IActorRef processingMessageSender = null;
            Receive<CreateNewProcess>(c =>
                                      {
                                          _log.Debug("Creating new process instance from {@message}", c);
                                          processingMessage = c.Message;
                                          processingMessageSender = Sender;
                                          pendingState = _processStateFactory.Create(processingMessage.Message, State);
                                          var cmd = new CreateNewStateCommand<TState>(pendingState.Id, pendingState);
                                          //will reply with CommandExecuted
                                          _stateAggregateActor.Tell(new MessageMetadataEnvelop<ICommand>(cmd, processingMessage.Metadata));
                                          Behavior.Become(AwaitingCreationConfirmationBehavior,nameof(AwaitingCreationConfirmationBehavior));
                                      });

            void AwaitingCreationConfirmationBehavior()
            {
                Receive<Status.Failure>(f => FinishWithError(processingMessage, processingMessageSender, f.Cause));

                //from state aggregate actor after persist
                Receive<CommandExecuted>(c =>
                                         {
                                             _log.Debug("Process instance created by message {@processResult}", processingMessage);

                                             if(Id != pendingState.Id)
                                             {
                                                 _log.Debug("Redirecting message to newly created process state instance, {id}", pendingState.Id);
                                                 var redirect = new MessageMetadataEnvelop(new ProcessRedirect(pendingState.Id, processingMessage), processingMessage.Metadata);
                                                 //requesting redirect from parent - persistence hub 
                                                 Context.Parent.Tell(redirect, processingMessageSender);
                                                 Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                                                 return;
                                             }
                                             State = pendingState;
                                             Self.Tell(processingMessage, processingMessageSender);
                                             Behavior.Become(TransitingProcessBehavior, nameof(TransitingProcessBehavior));
                                             pendingState = null;
                                             processingMessage = null;
                                             processingMessageSender = null;
                                         });
                StashingMessagesToProcessBehavior("process is waiting for process instance creation");
            }
        }

        private void TransitingProcessBehavior()
        {
            IMessageMetadataEnvelop processingEnvelop = null;
            TState pendingState = null;
            IReadOnlyCollection<ICommand> producedCommands = null;
            IActorRef processingMessageSender = null;

            Receive<IMessageMetadataEnvelop>(messageEnvelop =>
                                             {
                                                 _log.Debug("Transiting process by {@message}", messageEnvelop);

                                                 processingEnvelop = messageEnvelop;
                                                 processingMessageSender = Sender;
                                                 Behavior.Become(AwaitingTransitionConfirmationBehavior, nameof(AwaitingTransitionConfirmationBehavior));
                                                 Process.Transit(State, messageEnvelop.Message)
                                                        .PipeTo(Self);
                                             });

            void AwaitingTransitionConfirmationBehavior()
            {
                Receive<ProcessResult<TState>>(transitionResult =>
                                               {
                                                   _log.Debug("Process was transited, new state is {@state}", transitionResult.State);

                                                   pendingState = transitionResult.State;
                                                   producedCommands = transitionResult.ProducedCommands;
                                                   var cmd = new SaveStateCommand<TState>(Id,
                                                       pendingState,
                                                       State.CurrentStateName,
                                                       processingEnvelop);
                                                   //will reply back with CommandExecuted
                                                   _stateAggregateActor.Tell(new MessageMetadataEnvelop<ICommand>(cmd, processingEnvelop.Metadata));
                                               });
                Receive<CommandExecuted>(c =>
                                         {
                                             State = pendingState;
                                             processingMessageSender.Tell(new ProcessTransited(producedCommands,processingEnvelop.Metadata,
                                                                          _producedCommand,
                                                                          State));
                                             _log.Debug("Process dispatched {count} commands {@commands}", producedCommands?.Count ?? 0, producedCommands);

                                             Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                                             pendingState = null;
                                             processingMessageSender = null;
                                             processingEnvelop = null;
                                             producedCommands = null;
                                         });

                Receive<Status.Failure>(f => FinishWithError(processingEnvelop, processingMessageSender, f.Cause));

                StashingMessagesToProcessBehavior("process is waiting for transition confirmation");
            }
        }

        private void FinishWithError(IMessageMetadataEnvelop processingMessage, IActorRef messageSender, Exception error)
        {
            _log.Error(error, "Error during execution of message {@message}", processingMessage);

            var processorType = Process?.GetType() ?? typeof(TState);
            var fault = (IFault) Fault.NewGeneric(processingMessage.Message, error.UnwrapSingle(), Id, processorType);

            var faultMetadata = processingMessage.Metadata.CreateChild(fault.ProcessId, _exceptionOnTransit);

            _publisher.Publish(fault, faultMetadata);

            messageSender.Tell(new ProcessFault(fault, processingMessage.Metadata));

            Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
        }

        private Guid GetProcessId(object msg)
        {
            switch (msg)
            {
                case IFault f: return f.ProcessId;
                case IHaveProcessId p: return p.ProcessId;
            }
            throw new CannotGetProcessIdFromMessageException(msg);
        }
    }
}