using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.Sagas.Exceptions;
using GridDomain.Node.Actors.Sagas.Messages;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors.Sagas
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
                                                   SagaActorConstants.CreatedFaultForSagaTransit,
                                                   SagaActorConstants.SagaTransitCasedAndError);

            _sagaProducedCommand = new ProcessEntry(Self.Path.Name,
                                                    SagaActorConstants.PublishingCommand,
                                                    SagaActorConstants.SagaProducedACommand);


            var stateActorProps = Context.DI()
                                         .Props(typeof(SagaStateActor<TState>));
            var stateActor = Context.ActorOf(stateActorProps,
                                             AggregateActorName.New<SagaStateAggregate<TState>>(Id)
                                                               .Name);

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

                                           if (ss.State != null)
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
            ReceiveAsync<IMessageMetadataEnvelop>(env =>
                                                  {
                                                      Monitor.IncrementMessagesReceived();
                                                      if (_сreatorCatalog.CanCreateFrom(env.Message))
                                                          return StartNewSaga(env);

                                                      return TransitSaga(env.Message, env.Metadata);
                                                  });

            ReceiveAsync<RedirectToNewSaga>(env => StartNewSaga(env.MessageToRedirect, env.SagaId));

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
                                                  Become(() => Receive<Terminated>(t => Context.Stop(Self),
                                                                                   t => t.ActorRef.Path == _stateAggregateActor.Path));
                                                  _stateAggregateActor.Tell(r);
                                              });


            ReceiveAsync<CheckHealth>(s => _stateAggregateActor.Ask<HealthStatus>(s)
                                                               .PipeTo(Sender));

            Receive<NotifyOnPersistenceEvents>(c => _stateAggregateActor.Tell(c, Sender));
        }

        private Task StartNewSaga(IMessageMetadataEnvelop envelop, Guid? enforcedSagaId = null)
        {
            var message = envelop.Message;
            var metadata = envelop.Metadata;

            if(Saga != null)
                throw new SagaAlreadyStartedException(Saga.State, message);

            var saga = _сreatorCatalog.CreateNew(message, enforcedSagaId);
            if (Id != saga.State.Id)
            {
                FinishSagaTransition(new RedirectToNewSaga(saga.State.Id, envelop));
                return Task.CompletedTask;
            }

            var cmd = new CreateNewStateCommand<TState>(Id, saga.State);
            return PersistState(cmd, metadata)
                .ContinueWith(t =>
                              {
                                  if (t.IsFaulted)
                                      throw t.Exception;
                                  return TransitSaga(cmd, metadata);
                              })
                .Unwrap()
                .PipeTo(Self);
        }

        private async Task<CommandCompleted> PersistState(ISagaStateCommand<TState> cmd, IMessageMetadata messageMetadata)
        {
            var completed = await _stateAggregateActor.Ask<CommandCompleted>(new MessageMetadataEnvelop<ICommand>(cmd, messageMetadata));
            Saga = _сreatorCatalog.Create(cmd.State);
            return completed;
        }

        private Task<SagaTransited> TransitSaga(object msg, IMessageMetadata metadata)
        {
            if (Saga == null)
            {
                Log.Warning("Saga {saga} {sagaid} is not started but received transition message {@message} with metadata {@metadata}. Saga will not proceed. ", typeof(TState), Id, msg, metadata);
                return Task.FromResult(new SagaTransited(null, null, null, null));
            }
            if (Saga.State.Id != GetSagaId(msg) && !_сreatorCatalog.CanCreateFrom(msg))
            {
                Log.Error("Existing saga {saga} {sagaid} received message {@message} targeting different saga. Saga will not proceed.", typeof(TState), Saga.State.Id, msg);
            }
            //block any other executing until saga completes transition
            //cast is need for dynamic call of Transit

            Log.Warning("transiting saga by message " + msg);
            Task<TransitionResult<TState>> processSagaTask = Saga.PreviewTransit((dynamic) msg);

            return processSagaTask.ContinueWith(t => new SagaTransited(t.Result.ProducedCommands.ToArray(),
                                                                       metadata,
                                                                       _sagaProducedCommand,
                                                                       t.Result.State,
                                                                       t.Exception));
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Usially it is domain event or fault</param>
        /// <param name="messageMetadata"></param>
        private void AwaitingTransitionBehavior(object message, IMessageMetadata messageMetadata)
        {
            StashingMessagesToProcessBehavior();
            ReceiveAsync<SagaTransited>(t =>
                                        {
                                            var cmd = new SaveStateCommand<TState>(Id,
                                                                                   (TState) t.NewSagaState,
                                                                                   Saga.State.CurrentStateName,
                                                                                   message);

                                            return PersistState(cmd, messageMetadata)
                                                   .ContinueWith(tp =>
                                                                 {
                                                                     if (tp.IsFaulted)
                                                                         throw tp.Exception;
                                                                     FinishSagaTransition(t);
                                                                 });
                                        });

            Receive<Status.Failure>(f =>
                                    {
                                        var fault = PublishError(message,
                                                                 messageMetadata,
                                                                 f.Cause.UnwrapSingle());

                                        FinishSagaTransition(new SagaTransitFault(fault, messageMetadata));
                                    });
        }

        private void FinishSagaTransition(ISagaTransitCompleted message)
        {
            //notify saga process actor that saga transit is done
            foreach (var n in _sagaTransitionWaiters)
                n.Tell(message);

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