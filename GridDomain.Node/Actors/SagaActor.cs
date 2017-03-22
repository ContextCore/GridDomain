using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.CommandPipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.DI.Core;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    //TODO: add status info, e.g. was any errors during execution or recover

    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class SagaActor<TState> : ReceiveActor,
                                     IWithUnboundedStash where TState : class, ISagaState
    {
        private readonly ProcessEntry _exceptionOnTransit;
        private readonly ISagaProducer<TState> _producer;
        private readonly IPublisher _publisher;
        private readonly Dictionary<Type, string> _sagaIdFields;
        private readonly ProcessEntry _sagaProducedCommand;
        private readonly HashSet<Type> _sagaStartMessageTypes;
        private readonly ProcessEntry _stateChanged;
        private readonly ILoggingAdapter Log;
        private readonly IDictionary<Guid, TState> _persistancePendingStates = new Dictionary<Guid, TState>();
        private readonly BehaviorStack Behavior;
        private readonly ActorMonitor Monitor;
        private readonly IActorRef _stateAggregateActor;
        private readonly List<IActorRef> _sagaTransitionWaiters = new List<IActorRef>();

        public ISaga<TState> Saga { get; set; }

        public IStash Stash { get; set; }

        private Guid Id { get; }

        public SagaActor(ISagaProducer<TState> producer,
                         IPublisher publisher)

        {
            Monitor = new ActorMonitor(Context, "Saga" + typeof(TState).Name);
            Behavior = new BehaviorStack(BecomeStacked, UnbecomeStacked);

            Guid id;
            if (!AggregateActorName.TryParseId(Self.Path.Name, out id))
                throw new BadNameFormatException();
            Id = id;

            _publisher = publisher;
            _producer = producer;
            Log = Context.GetLogger();
            _sagaStartMessageTypes = new HashSet<Type>(producer.KnownDataTypes.Where(t => typeof(DomainEvent).IsAssignableFrom(t)));
            _sagaIdFields = producer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType, m => m.CorrelationField);

            _exceptionOnTransit = new ProcessEntry(Self.Path.Name,
                                                   SagaActorLiterals.CreatedFaultForSagaTransit,
                                                   SagaActorLiterals.SagaTransitCasedAndError);

            _sagaProducedCommand = new ProcessEntry(Self.Path.Name,
                                                    SagaActorLiterals.PublishingCommand,
                                                    SagaActorLiterals.SagaProducedACommand);

            _stateChanged = new ProcessEntry(Self.Path.Name, "Saga state event published", "Saga changed state");

            var stateActorProps = Context.DI().Props(typeof(SagaStateActor<TState>));
            var stateActor = Context.ActorOf(stateActorProps, AggregateActorName.New<SagaStateAggregate<TState>>(Id).Name);

            _stateAggregateActor = stateActor;
            _stateAggregateActor.Tell(NotifyOnCommandComplete.Instance);
            _stateAggregateActor.Tell(GetSagaState.Instance);

            Behavior.Become(InitializingBehavior, nameof(InitializingBehavior));
        }

        private void InitializingBehavior()
        {
            bool commandsSubscribed = false;
            bool stateInitialized = false;

            Receive<NotifyOnCommandCompletedAck>(a =>
                                                 {
                                                     commandsSubscribed = true;
                                                     if (commandsSubscribed && stateInitialized)
                                                         FinishInitialization();
                                                 });
            Receive<SagaState<TState>>(ss =>
                                       {
                                           stateInitialized = true;
                                           Saga = _producer.Create(ss.State);
                                           if (commandsSubscribed && stateInitialized)
                                               FinishInitialization();
                                       });

            StashingMessagesToProcessBehavior();
        }

        private void FinishInitialization()
        {
            Behavior.Unbecome();
            Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
            Stash.UnstashAll();
        }

        private void StashingMessagesToProcessBehavior()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(m => Stash.Stash(),
                                                          e => GetSagaId(e.Message) == Id);

            Receive<IMessageMetadataEnvelop<IFault>>(m => Stash.Stash(),
                                                     e => GetSagaId(e.Message) == Id);

            ProxifyingCommandsBehavior();
        }

        private void AwaitingMessageBehavior()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(m => ProcessMessage(m.Message, m.Metadata),
                                                          e => GetSagaId(e.Message) == Id);

            Receive<IMessageMetadataEnvelop<IFault>>(m => ProcessMessage(m.Message, m.Metadata),
                                                     e => GetSagaId(e.Message) == Id);

            ProxifyingCommandsBehavior();
        }

        private void ProxifyingCommandsBehavior()
        {
            Receive<NotifyOnSagaTransited>(m =>
                                           {
                                               _sagaTransitionWaiters.Add(m.Sender);
                                               Sender.Tell(NotifyOnSagaTransitedAck.Instance);
                                           });

            Receive<GracefullShutdownRequest>(r =>
                                              {
                                                  _stateAggregateActor.Tell(r);
                                                  Context.Stop(Self);
                                              });


            Receive<CheckHealth>(s =>
                                 {
                                     var sender = Sender;
                                     _stateAggregateActor.Ask<HealthStatus>(s).ContinueWith(t => sender.Tell(t.Result));
                                 });

            Receive<NotifyOnPersistenceEvents>(c => _stateAggregateActor.Tell(c));
        }

        private void ProcessMessage(object message, IMessageMetadata metadata)
        {
            var sender = Sender;
            Monitor.IncrementMessagesReceived();
            if (ShouldStartNewSaga(message))
                StartNewSaga(message, metadata, sender);
            else
                TransitSaga(message, metadata, sender);
        }

        private void StartNewSaga(object message, IMessageMetadata metadata, IActorRef sender)
        {
            var saga = _producer.Create(message);

            var cmd = new CreateNewStateCommand<TState>(Id, saga.State);

            ChangeState(cmd, metadata, () => TransitSaga(message, metadata, sender));
        }

        private void WaitForCommandCompleteBehavior(Action<Guid> onCommandComplete)
        {
            StashingMessagesToProcessBehavior();
            Receive<CommandCompleted>(cc => onCommandComplete(cc.CommandId));
        }

        private bool ShouldStartNewSaga(object message)
        {
            return _sagaStartMessageTypes.Contains(message.GetType());
        }

        private void ChangeState(ISagaStateCommand<TState> cmd, IMessageMetadata messageMetadata, Action onStateChanged)
        {
            _persistancePendingStates[cmd.Id] = cmd.State;

            _stateAggregateActor.Ask<CommandCompleted>(new MessageMetadataEnvelop<ICommand>(cmd, messageMetadata))
                                .PipeTo(Self);

            Behavior.Become(() => WaitForCommandCompleteBehavior(id =>
                                                                 {
                                                                     Behavior.Unbecome();
                                                                     Saga = ApplyPendingState(id);
                                                                     onStateChanged();
                                                                 }),
                            nameof(WaitForCommandCompleteBehavior));
        }

        private ISaga<TState> ApplyPendingState(Guid id)
        {
            TState persistedState;
            if (!_persistancePendingStates.TryGetValue(id, out persistedState))
                throw new UnknownStatePersistedException();
            _persistancePendingStates.Remove(id);

            return _producer.Create(persistedState);
        }

        private void TransitSaga(object msg, IMessageMetadata metadata, IActorRef sender)
        {
            //block any other executing until saga completes transition
            //cast is need for dynamic call of Transit
            Task<TransitionResult<TState>> processSagaTask = Saga.PreviewTransit((dynamic) msg);
            processSagaTask.ContinueWith(t => new SagaTransited(t.Result.ProducedCommands.ToArray(),
                                                                metadata,
                                                                _sagaProducedCommand,
                                                                t.Result.State,
                                                                t.Exception))
                           .PipeTo(Self, sender);

            Behavior.Become(() => AwaitingTransitionBehavior(msg, metadata), nameof(AwaitingTransitionBehavior));
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Usially it is domain event or fault</param>
        /// <param name="messageMetadata"></param>
        private void AwaitingTransitionBehavior(object message, IMessageMetadata messageMetadata)
        {
            StashingMessagesToProcessBehavior();
            Receive<SagaTransited>(t =>
                                   {
                                       var cmd = new SaveStateCommand<TState>(Id,
                                                                              (TState) t.NewSagaState,
                                                                              Saga.State.CurrentStateName, message);
                                       var sender = Sender;
                                       ChangeState(cmd, messageMetadata, () => FinishSagaTransition(t, sender));
                                   });

            Receive<Status.Failure>(f =>
                                    {
                                        var sender = Sender;
                                        var fault = PublishError(message,
                                                                 messageMetadata,
                                                                 f.Cause.UnwrapSingle());

                                        FinishSagaTransition(new SagaTransitFault(fault, messageMetadata),sender);
                                    });
        }

        private void FinishSagaTransition(ISagaTransitCompleted message, IActorRef sender)
        {
            //notify saga process actor that saga transit is done
            foreach (var n in _sagaTransitionWaiters)
                n.Tell(message);
            sender.Tell(message);

            Stash.UnstashAll();
            Behavior.Unbecome();
        }

        private IFault PublishError(object message, IMessageMetadata messageMetadata, Exception exception)
        {
            var processorType = _producer.Descriptor.StateMachineType;

            Log.Error(exception, "Saga {saga} {id} raised an error on {@message}", processorType, Id, message);
            var fault = Fault.NewGeneric(message, exception, Id, processorType);

            var metadata = messageMetadata.CreateChild(fault.SagaId, _exceptionOnTransit);

            _publisher.Publish(fault, metadata);
            return fault;
        }

        private Guid GetSagaId(object msg)
        {
            Guid sagaId = Guid.Empty;
            msg.Match()
               .With<IFault>(m => sagaId = m.SagaId)
               .With<IHaveSagaId>(m => sagaId = m.SagaId)
               .Default(m =>
                        {
                            var type = msg.GetType();
                            string fieldName;
                            if (_sagaIdFields.TryGetValue(type, out fieldName))
                                sagaId = (Guid) type.GetProperty(fieldName).GetValue(msg);
                            else
                                throw new CannotFindSagaIdException(msg);
                        });

            return sagaId;
        }
    }

    internal class NotifyOnSagaTransited
    {
        public NotifyOnSagaTransited(IActorRef sender)
        {
            Sender = sender;
        }

        public IActorRef Sender { get; }
    }

    internal class NotifyOnSagaTransitedAck
    {
        private NotifyOnSagaTransitedAck() {}

        public static readonly NotifyOnSagaTransitedAck Instance = new NotifyOnSagaTransitedAck();
    }

    internal class UnknownStatePersistedException : Exception {}

    public class CommandCompleted
    {
        public Guid CommandId { get; }

        public CommandCompleted(Guid commandId)
        {
            CommandId = commandId;
        }

        public static CommandCompleted Instance { get; } = new CommandCompleted(Guid.Empty);
    }

    internal class CannotFindSagaIdException : Exception
    {
        public object Msg { get; }

        public CannotFindSagaIdException(object msg)
        {
            Msg = msg;
        }
    }
}