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
        private readonly IActorRef _stateAggregateActor;

        public IProcess<TState> Process { get; private set; }
        public TState State { get; private set; }

        public IStash Stash { get; set; }

        private Guid Id { get; }

        public ProcessActor(IProcess<TState> process, IProcessStateFactory<TState> processStateFactory)

        {
            Process = process;
            Monitor = new ActorMonitor(Context, "Process" + typeof(TState).Name);
            Behavior = new BehaviorQueue(Become);

            Guid id;
            if(!AggregateActorName.TryParseId(Self.Path.Name, out id))
                throw new BadNameFormatException();
            Id = id;

            _publisher = Context.System.GetTransport();
            _processStateFactory = processStateFactory;
            _log = Context.GetSeriLogger();

            _exceptionOnTransit = ProcessManagerActorConstants.ExceptionOnTransit(Self.Path.Name);
            _producedCommand = ProcessManagerActorConstants.ProcessProduceCommands(Self.Path.Name);

            var stateActorProps = Context.DI()
                                         .Props(typeof(ProcessStateActor<TState>));

            _stateAggregateActor = Context.ActorOf(stateActorProps,
                                                   AggregateActorName.New<ProcessStateAggregate<TState>>(Id)
                                                                     .Name);
            _stateAggregateActor.Tell(GetProcessState.Instance);
            Behavior.Become(InitializingBehavior, nameof(InitializingBehavior));
        }

        private void StashingMessagesToProcessBehavior()
        {
            Receive<IMessageMetadataEnvelop>(m => StashMessage(m));
            Receive<ProcessRedirect>(m => StashMessage(m));
            ProxifyingCommandsBehavior();
        }

        private void InitializingBehavior()
        {
            Receive<ProcesStateMessage<TState>>(ss =>
                                                {
                State = ss.State;
                FinishInitialization();
            });

            StashingMessagesToProcessBehavior();
        }

        private void FinishInitialization()
        {
            Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
            Stash.UnstashAll();
        }

        private void StashMessage(object m)
        {
            _log.Warning("Stashing message {message}", m);
            Stash.Stash();
        }

        private void AwaitingMessageBehavior()
        {
            Receive<IMessageMetadataEnvelop>(env =>
                                            {
                                                 var state = _processStateFactory.Create(env.Message, State);
                                                 if(State == null)
                                                 {
                                                     Self.Tell(new CreateNewProcess(env), Sender);
                                                     Behavior.Become(CreatingProcessBehavior, nameof(CreatingProcessBehavior));
                                                 }
                                                 else
                                                 {
                                                     Self.Tell(env, Sender);
                                                     Behavior.Become(TransitingProcessBehavior, nameof(TransitingProcessBehavior));
                                                 }
                                                State = state;
                                            });

            Receive<ProcessRedirect>(env =>
            {
                Self.Tell(new CreateNewProcess(env.MessageToRedirect, env.ProcessId), Sender);
                Behavior.Become(CreatingProcessBehavior, nameof(CreatingProcessBehavior));
            });

            ProxifyingCommandsBehavior();
        }

        private void ProxifyingCommandsBehavior()
        {
            Receive<GracefullShutdownRequest>(r =>
            {
                _log.Debug("Received shutdown request");
                Context.Watch(_stateAggregateActor);
                Become(() => Receive<Terminated>(t => Context.Stop(Self),
                                                 t => t.ActorRef.Path == _stateAggregateActor.Path));
                _stateAggregateActor.Tell(r);
            });


            ReceiveAsync<CheckHealth>(s => _stateAggregateActor.Ask<HealthStatus>(s)
                                                               .PipeTo(Sender));

            Receive<NotifyOnPersistenceEvents>(c => _stateAggregateActor.Tell(c, Sender));
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

                pendingState = _processStateFactory.Create(processingMessage.Message,State);
                if(Id != pendingState.Id)
                {
                    FinishProcessTransition(new ProcessRedirect(pendingState.Id, processingMessage), Sender);
                    return;
                }

                if(Process != null)
                    throw new ProcessAlreadyStartedException(State, processingMessage);

                var cmd = new CreateNewStateCommand<TState>(Id, pendingState);

                _stateAggregateActor.Ask<CommandExecuted>(new MessageMetadataEnvelop<ICommand>(cmd, processingMessage.Metadata))
                                    .PipeTo(Self);
            });

            Receive<Status.Failure>(f => FinishWithError(processingMessage, processingMessageSender, f.Cause));

            //from state aggregate actro after persist
            Receive<CommandExecuted>(c =>
            {
                _log.Debug("Process instance created by message {@processResult}", processingMessage);
                State = pendingState;
                Self.Tell(processingMessage, processingMessageSender);
                Behavior.Become(TransitingProcessBehavior, nameof(TransitingProcessBehavior));
                pendingState = null;
                processingMessage = null;
                processingMessageSender = null;
            });
            StashingMessagesToProcessBehavior();

        }

        private void TransitingProcessBehavior()
        {
            IMessageMetadataEnvelop processingEnvelop = null;
            TState pendingState = null;
            IReadOnlyCollection<ICommand> producedCommands = new ICommand[] { };
            IActorRef processingMessageSender = null;

            Receive<IMessageMetadataEnvelop>(messageEnvelop =>
            {
                if(Id != GetProcessId(messageEnvelop.Message))
                {
                    _log.Error("Received message {@message} "
                               + "targeting different process. Process will not proceed.",
                               messageEnvelop);

                    FinishWithError(messageEnvelop, Sender, new ProcessIdMismatchException());
                    return;
                }
                if(Process == null)
                {
                    _log.Error("Process is not started but received transition message {@message}. "
                               + "Process will not proceed. ", typeof(TState), Id, messageEnvelop);
                    Task.FromException(new ProcessNotStartedException()).PipeTo(Self);
                    return;
                }

                _log.Debug("Transiting process by {@message}", messageEnvelop);

                processingEnvelop = messageEnvelop;
                processingMessageSender = Sender;

                Process.Transit(messageEnvelop.Message, State).PipeTo(Self);
            });

            ReceiveAsync<ProcessResult<TState>>(transitionResult =>
            {
                _log.Debug("Process state is mutating with command {@processResult}", transitionResult);

                pendingState = transitionResult.State;
                producedCommands = transitionResult.ProducedCommands;
                var cmd = new SaveStateCommand<TState>(Id,
                                                       pendingState,
                                                       State.CurrentStateName,
                                                       processingEnvelop);

                return _stateAggregateActor.Ask<CommandExecuted>(new MessageMetadataEnvelop<ICommand>(cmd, processingEnvelop.Metadata))
                                           .PipeTo(Self);
            });
            Receive<CommandExecuted>(c =>
            {
                State = pendingState;
                FinishProcessTransition(new ProcessTransited(producedCommands.ToArray(),
                                                       processingEnvelop.Metadata,
                                                       _producedCommand,
                                                       pendingState),
                                     processingMessageSender);
                Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                pendingState = null;
                processingMessageSender = null;
                processingEnvelop = null;
                producedCommands = null;
                _log.Debug("Process state mutated with command {@commandCompleted}", c);
            });

            Receive<Status.Failure>(f => FinishWithError(processingEnvelop, processingMessageSender, f.Cause));

            StashingMessagesToProcessBehavior();
        }

        private void FinishWithError(IMessageMetadataEnvelop processingMessage, IActorRef messageSender, Exception error)
        {
            _log.Error(error, "Error during execution of message {@message}", processingMessage);

            var processorType = Process?.GetType() ?? typeof(TState);
            var fault = (IFault)Fault.NewGeneric(processingMessage.Message, error.UnwrapSingle(), Id, processorType);

            var faultMetadata = processingMessage.Metadata.CreateChild(fault.ProcessId, _exceptionOnTransit);

            _publisher.Publish(fault, faultMetadata);

            FinishProcessTransition(new ProcessFault(fault, processingMessage.Metadata), messageSender);

            Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
        }

        private void FinishProcessTransition(IProcessCompleted message, IActorRef messageSender)
        {
            messageSender.Tell(message);
            Stash.Unstash();
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

    internal class CannotGetProcessIdFromMessageException : Exception
    {
        public object Message { get; }

        public CannotGetProcessIdFromMessageException(object msg)
        {
            Message = msg;
        }
    }
}