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

        private string Id { get; }

        class ProcessExecutionContext
        {
            public TState PendingState { get; private set;}
            public IMessageMetadataEnvelop ProcessingMessage { get;private set;}
            public IActorRef ProcessingMessageSender { get; private set; }

            public bool IsFinished => PendingState == null;
            public bool IsInitializing { get; set; }

            public void StartNewExecution(TState pendingState, IMessageMetadataEnvelop msg, IActorRef sender)
            {
                if(IsInitializing) 
                    throw new InvalidOperationException("Cannot start new execution while initializing");

                PendingState = pendingState ?? throw new ProcessStateNullException();
                ProcessingMessage = msg;
                ProcessingMessageSender = sender;
            }
            
            public void Clear()
            {
                PendingState = null;
                ProcessingMessage = null;
                ProcessingMessageSender = null;
                IsInitializing = false;
            }
        }

        private ProcessExecutionContext ExecutionContext { get; } = new ProcessExecutionContext();
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
            ExecutionContext.IsInitializing = true;
            
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
                                                    ExecutionContext.IsInitializing = false;
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
            Receive<Shutdown.Request>(r =>
                                              {
                                                  if (ExecutionContext.IsInitializing)
                                                  {
                                                      _log.Debug("Process gracefull shutdown request declined. Waiting initializtion to finish");
                                                      return;
                                                  }
                                                  
                                                  if (!ExecutionContext.IsFinished)
                                                  {
                                                      _log.Debug("Process gracefull shutdown request declined. Waiting process execution to finish");
                                                      return;
                                                  }
                                                  
                                                   _log.Debug("Terminating process due to gracefull shutdown request");
                                                   Context.Stop(Self);
                                              });


            Receive<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));

            Receive<NotifyOnPersistenceEvents>(c => ((ICanTell)_stateAggregateActor ?? _stateActorSelection).Tell(c, Sender));
        }

       
        private void CreatingProcessBehavior()
        {

            Receive<CreateNewProcess>(c =>
                                      {
                                          _log.Debug("Creating new process instance from {@message}", c);
                                          var pendingState = _processStateFactory.Create(c.Message.Message);

                                          ExecutionContext.StartNewExecution(pendingState, c.Message, Sender);
                                        
                                          var cmd = new CreateNewStateCommand<TState>(ExecutionContext.PendingState.Id, ExecutionContext.PendingState);
                                          //will reply with CommandExecuted
                                          _stateAggregateActor.Tell(new MessageMetadataEnvelop<ICommand>(cmd, ExecutionContext.ProcessingMessage.Metadata));
                                          Behavior.Become(AwaitingCreationConfirmationBehavior,nameof(AwaitingCreationConfirmationBehavior));
                                      });

            void AwaitingCreationConfirmationBehavior()
            {
                Receive<Status.Failure>(f => FinishWithError(ExecutionContext.ProcessingMessage, ExecutionContext.ProcessingMessageSender, f.Cause));

                //from state aggregate actor after persist
                Receive<CommandExecuted>(c =>
                                         {
                                             _log.Debug("Process instance created by message {@processResult}", ExecutionContext.ProcessingMessage);

                                             var pendingStateId = ExecutionContext.PendingState.Id;
                                             if(Id != pendingStateId)
                                             {
                                                 _log.Debug("Redirecting message to newly created process state instance, {id}", pendingStateId);
                                                 var redirect = new MessageMetadataEnvelop(new ProcessRedirect(pendingStateId, ExecutionContext.ProcessingMessage), ExecutionContext.ProcessingMessage.Metadata);
                                                 //requesting redirect from parent - persistence hub 
                                                 Context.Parent.Tell(redirect, ExecutionContext.ProcessingMessageSender);
                                                 Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                                                 ExecutionContext.Clear();
                                                 return;
                                             }
                                             
                                             State = ExecutionContext.PendingState;
                                             Self.Tell(ExecutionContext.ProcessingMessage, ExecutionContext.ProcessingMessageSender);
                                             Behavior.Become(TransitingProcessBehavior, nameof(TransitingProcessBehavior));
                                             ExecutionContext.Clear();
                                         });
                StashingMessagesToProcessBehavior("process is waiting for process instance creation");
            }
        }

        private void TransitingProcessBehavior()
        {
            IMessageMetadataEnvelop processingEnvelop = null;
            IReadOnlyCollection<ICommand> producedCommands = null;
            IActorRef processingMessageSender = null;

            Receive<IMessageMetadataEnvelop>(messageEnvelop =>
                                             {
                                                 _log.Debug("Transiting process by {@message}", messageEnvelop);

                                                 processingEnvelop = messageEnvelop;
                                                 processingMessageSender = Sender;
                                                 var pendingState = (TState)State.Clone();
                                                 Behavior.Become(() => AwaitingTransitionConfirmationBehavior(pendingState), nameof(AwaitingTransitionConfirmationBehavior));
                                                 Process.Transit(pendingState, messageEnvelop.Message)
                                                        .PipeTo(Self);
                                             });

            void AwaitingTransitionConfirmationBehavior(TState pendingState)
            {
                Receive<IReadOnlyCollection<ICommand>>(transitionResult =>
                                               {
                                                   _log.Debug("Process was transited, new state is {@state}", pendingState);
                                                   producedCommands = transitionResult;
                                                   var cmd = new SaveStateCommand<TState>(Id,
                                                                                          pendingState,
                                                                                          GetMessageId(processingEnvelop));
                                                   //will reply back with CommandExecuted
                                                   _stateAggregateActor.Tell(new MessageMetadataEnvelop<SaveStateCommand<TState>>(cmd, processingEnvelop.Metadata));
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

        private static string GetMessageId(IMessageMetadataEnvelop processingEnvelop)
        {
            switch (processingEnvelop.Message)
            {
                case IHaveId e: return e.Id;
                case IFault<ICommand> e: return e.Message.Id;
            }
            throw new CannotGetProcessIdFromMessageException(processingEnvelop);
        }

        private void FinishWithError(IMessageMetadataEnvelop processingMessage, IActorRef messageSender, Exception error)
        {
            _log.Error(error, "Error during execution of message {@message}", processingMessage);

            var processorType = Process?.GetType() ?? typeof(TState);
            var fault = (IFault) Fault.NewGeneric(processingMessage.Message, error.UnwrapSingle(), Id, processorType);

            var faultMetadata = MessageMetadataExtensions.CreateChild(processingMessage.Metadata, (string) fault.ProcessId, _exceptionOnTransit);

            _publisher.Publish(fault, faultMetadata);

            messageSender.Tell(new ProcessFault(fault, processingMessage.Metadata));

            Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
            ExecutionContext.Clear();
        }

        private string GetProcessId(object msg)
        {
            switch (msg)
            {
                case IFault f: return f.ProcessId;
                case IHaveProcessId p: return p.ProcessId;
            }
            throw new CannotGetProcessIdFromMessageException(msg);
        }
    }

    internal class ProcessStateNullException : Exception
    {
        public ProcessStateNullException():base("Process state, produced by state factory is null, check factory for possible errors")
        {
            
        }
    }
}