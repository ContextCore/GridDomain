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
        private readonly ISagaCreatorCatalog<TState> _сreatorCatalog;
        private readonly IPublisher _publisher;
        private readonly ProcessEntry _sagaProducedCommand;
        private readonly ILoggingAdapter Log;
        private readonly IDictionary<Guid, TState> _persistancePendingStates = new Dictionary<Guid, TState>();
        private readonly BehaviorStack Behavior;
        private readonly ActorMonitor Monitor;
        private readonly IActorRef _stateAggregateActor;
        private readonly List<IActorRef> _sagaTransitionWaiters = new List<IActorRef>();

        public ISaga<TState> Saga { get; private set; }

        public IStash Stash { get; set; }

        private Guid Id { get; }

        public SagaActor(ISagaCreatorCatalog<TState> сreatorCatalog,
                         IPublisher publisher)

        {
            Monitor = new ActorMonitor(Context, "Saga" + typeof(TState).Name);
            Behavior = new BehaviorStack(BecomeStacked, UnbecomeStacked);

            Guid id;
            if (!AggregateActorName.TryParseId(Self.Path.Name, out id))
                throw new BadNameFormatException();
            Id = id;

            _publisher = publisher;
            _сreatorCatalog = сreatorCatalog;
            Log = Context.GetLogger();

            _exceptionOnTransit = new ProcessEntry(Self.Path.Name,
                                                   SagaActorLiterals.CreatedFaultForSagaTransit,
                                                   SagaActorLiterals.SagaTransitCasedAndError);

            _sagaProducedCommand = new ProcessEntry(Self.Path.Name,
                                                    SagaActorLiterals.PublishingCommand,
                                                    SagaActorLiterals.SagaProducedACommand);


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

                                           if(ss.State != null)
                                              Saga = _сreatorCatalog.Create(ss.State);

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
            Receive<IMessageMetadataEnvelop<DomainEvent>>(m => StashMessage(m));

            Receive<IMessageMetadataEnvelop<IFault>>(m => StashMessage(m));

            Receive<RedirectToNewSaga>(m => StashMessage(m));

            ProxifyingCommandsBehavior();
        }

        private void StashMessage(object m)
        {
            Log.Warning("Saga {id} stashing message {messge}", Id, m);
            Stash.Stash();
        }

        private void AwaitingMessageBehavior()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(env => ProcessMessage(env));

            Receive<IMessageMetadataEnvelop<IFault>>(env => ProcessMessage(env));

            Receive<RedirectToNewSaga>(env => StartNewSaga(Sender,env.MessageToRedirect, env.SagaId));

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
                                                  Context.Watch(_stateAggregateActor);
                                                  Become( () => Receive<Terminated>( t => Context.Stop(Self),
                                                            t => t.ActorRef.Path == _stateAggregateActor.Path));
                                                  _stateAggregateActor.Tell(r);
                                              });


            ReceiveAsync<CheckHealth>(s => _stateAggregateActor.Ask<HealthStatus>(s).PipeTo(Sender));

            Receive<NotifyOnPersistenceEvents>(c => _stateAggregateActor.Tell(c,Sender));
        }

        private void ProcessMessage(IMessageMetadataEnvelop envelop)
        {
            var message = envelop.Message;
            var metadata = envelop.Metadata;

            Monitor.IncrementMessagesReceived();
            if (_сreatorCatalog.CanCreateFrom(message))
            {
                StartNewSaga(Sender, envelop);
            }
            else
                TransitSaga(message, metadata, Sender);
        }

        private void StartNewSaga(IActorRef sender, IMessageMetadataEnvelop envelop, Guid? enforcedSagaId = null )
        {
            var message = envelop.Message;
            var metadata = envelop.Metadata;
           
            var saga = _сreatorCatalog.CreateNew(message, enforcedSagaId);
            if (Id != saga.State.Id)
            {
                FinishSagaTransition(new RedirectToNewSaga(saga.State.Id, envelop), sender);
                Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                return;
            }

            if (Saga != null)
                throw new SagaAlreadyStartedException(Saga.State, message);

            var cmd = new CreateNewStateCommand<TState>(Id, saga.State);
            WaitCommandComplete(cmd, metadata, () => TransitSaga(message, metadata, sender)).PipeTo(Self);
        }

        private void WaitForCommandCompleteBehavior(Action<Guid> onCommandComplete)
        {
            StashingMessagesToProcessBehavior();
            Receive<CommandCompleted>(cc => onCommandComplete(cc.CommandId));
        }

        private Task<CommandCompleted> WaitCommandComplete(ISagaStateCommand<TState> cmd, IMessageMetadata messageMetadata, Action onStateChanged)
        {
            _persistancePendingStates[cmd.Id] = cmd.State;

            Behavior.Become(() => WaitForCommandCompleteBehavior(id =>
            {
                Behavior.Unbecome();
                Saga = ApplyPendingState(id);
                if (Saga.State.Id != Id)
                    throw new SagaStateException("Saga changed id during appling new state");

                onStateChanged();
            }),
                       nameof(WaitForCommandCompleteBehavior));

            return _stateAggregateActor.Ask<CommandCompleted>(new MessageMetadataEnvelop<ICommand>(cmd, messageMetadata));
        }

        private ISaga<TState> ApplyPendingState(Guid id)
        {
            TState persistedState;
            if (!_persistancePendingStates.TryGetValue(id, out persistedState))
                throw new UnknownStatePersistedException();
            _persistancePendingStates.Remove(id);

            return _сreatorCatalog.Create(persistedState);
        }

        private void TransitSaga(object msg, IMessageMetadata metadata, IActorRef sender)
        {
            if (Saga == null)
            {
                Log.Warning("Saga {saga} {sagaid} is not started but received transition message {@message} with metadata {@metadata}. Saga will not proceed. ", typeof(TState), Id, msg, metadata);
                return;
            }
            if (Saga.State.Id != GetSagaId(msg) && !_сreatorCatalog.CanCreateFrom(msg))
            {
                Log.Error("Existing saga {saga} {sagaid} received message {@message} targeting different saga. Saga will not proceed.", typeof(TState), Saga.State.Id, msg);

            }
            //block any other executing until saga completes transition
            //cast is need for dynamic call of Transit

            Behavior.Become(() => AwaitingTransitionBehavior(msg, metadata), nameof(AwaitingTransitionBehavior));

            Task<TransitionResult<TState>> processSagaTask = Saga.PreviewTransit((dynamic)msg);
            processSagaTask.ContinueWith(t => new SagaTransited(t.Result.ProducedCommands.ToArray(),
                                                                metadata,
                                                                _sagaProducedCommand,
                                                                t.Result.State,
                                                                t.Exception))
                           .PipeTo(Self, sender);
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
                                       WaitCommandComplete(cmd, messageMetadata, () => FinishSagaTransition(t, sender)).PipeTo(Self);
                                   });

            Receive<Status.Failure>(f => ProcessError(message, messageMetadata, f.Cause));
            Receive<Failure>(f => ProcessError(message, messageMetadata, f.Exception));
        }

        private void ProcessError(object message, IMessageMetadata messageMetadata, Exception error)
        {
            var fault = PublishError(message,
                                     messageMetadata,
                                     error.UnwrapSingle());

            FinishSagaTransition(new SagaTransitFault(fault, messageMetadata), Sender);
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
            var processorType = Saga?.GetType() ?? typeof(TState);

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
               .With<IHaveSagaId>(m => sagaId = m.SagaId);
            return sagaId;
        }
    }
}