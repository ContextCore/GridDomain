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
        private readonly ISagaProducer<ISaga<TState>> _producer;
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

        public ISaga<TState> Saga { get; set; }

        //private TState State { get; set; }
        public IStash Stash { get; set; }

        private Guid Id { get; }

        public SagaActor(Guid id,
                         ISagaProducer<ISaga<TState>> producer,
                         IPublisher publisher,
                         IActorRef aggregateActor)

        {
            Monitor = new ActorMonitor(Context, "Saga" + typeof(TState).Name);
            Behavior = new BehaviorStack(BecomeStacked, UnbecomeStacked);

            _stateAggregateActor = aggregateActor;
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

            _stateAggregateActor.Tell(NotifyOnCommandComplete.Instance);
            Behavior.Become(InitializingBehavior, nameof(InitializingBehavior));
        }

        private void InitializingBehavior()
        {
            Receive<NotifyOnCommandCompletedAck>(a =>
                                                 {
                                                     Behavior.Unbecome();
                                                     Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                                                     Stash.UnstashAll();
                                                 });

            StashingMessagesToProcessBehavior();
        }

        private void StashingMessagesToProcessBehavior()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(m => Stash.Stash(),
                                                          e => GetSagaId(e.Message) == Id);

            Receive<IMessageMetadataEnvelop<IFault>>(m => Stash.Stash(),
                                                     e => GetSagaId(e.Message) == Id);
        }

        private void AwaitingMessageBehavior()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(m => ProcessMessage(m.Message, m.Metadata),
                                                          e => GetSagaId(e.Message) == Id);

            Receive<IMessageMetadataEnvelop<IFault>>(m => ProcessMessage(m.Message, m.Metadata),
                                                     e => GetSagaId(e.Message) == Id);
        }

        private void ProcessMessage(object message, IMessageMetadata metadata)
        {
            Monitor.IncrementMessagesReceived();
            if (ShouldStartNewSaga(message))
                StartNewSaga(message, metadata);
            else
                TransitSaga(message, metadata);
        }

        private void StartNewSaga(object message, IMessageMetadata metadata)
        {
            var saga = _producer.Create(message);

            var cmd = new CreateNewStateCommand<TState>(Id, saga.State);

            ChangeState(cmd, metadata, () => TransitSaga(message, metadata));
        }

        private void WaitForCommandCompleteBehavior(Action<Guid> onCommandComplete)
        {
            StashingMessagesToProcessBehavior();
            Receive<CommandCompleted>(cc => { onCommandComplete(cc.CommandId); });
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
                                                                   Saga = ApplyPendingState(id);
                                                                   onStateChanged();
                                                                   Behavior.Unbecome();
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

        private void TransitSaga(object msg, IMessageMetadata metadata)
        {
            //block any other executing until saga completes transition
            //cast is need for dynamic call of Transit
            Task<TransitionResult<TState>> processSagaTask = Saga.PreviewTransit((dynamic) msg);
            processSagaTask.ContinueWith(t => new SagaTransited(t.Result.ProducedCommands.ToArray(),
                                                                metadata,
                                                                _sagaProducedCommand,
                                                                t.Result.State,
                                                                t.Exception))
                           .PipeTo(Self);

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

                                       ChangeState(cmd, messageMetadata, () => FinishMessageProcessing(t));
                                   });

            Receive<Status.Failure>(f =>
                                    {
                                        var fault = PublishError(message,
                                                                 messageMetadata,
                                                                 f.Cause.UnwrapSingle());

                                        FinishMessageProcessing(new SagaTransitFault(fault, messageMetadata));
                                    });
        }

        private void FinishMessageProcessing(object message)
        {
            //notify saga process actor that saga transit is done
            Context.Parent.Tell(message);
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
            var type = msg.GetType();
            string fieldName;

            if (_sagaIdFields.TryGetValue(type, out fieldName))
                return (Guid) type.GetProperty(fieldName).GetValue(msg);

            throw new CannotFindSagaIdException(msg);
        }
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