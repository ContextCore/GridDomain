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
        private readonly ProcessEntry _sagaProducedCommand;
        private readonly ISagaCreatorCatalog<TState> _сreatorCatalog;
        private readonly IPublisher _publisher;
        private readonly ILoggingAdapter _log;
        private BehaviorStack Behavior { get; }
        private ActorMonitor Monitor { get; }
        private readonly IActorRef _stateAggregateActor;
        private readonly List<IActorRef> _sagaTransitionWaiters = new List<IActorRef>();

        public ISaga<TState> Saga { get; private set; }

        public IStash Stash { get; set; }

        private Guid Id { get; }

        public SagaActor(ISagaCreatorCatalog<TState> сreatorCatalog,
                         IPublisher publisher)

        {
            Monitor = new ActorMonitor(Context, "Saga" + typeof(TState).Name);
            Behavior = new BehaviorStack(Become, UnbecomeStacked);

            Guid id;
            if (!AggregateActorName.TryParseId(Self.Path.Name, out id))
                throw new BadNameFormatException();
            Id = id;

            _publisher = publisher;
            _сreatorCatalog = сreatorCatalog;
            _log = Context.GetLogger();

            _exceptionOnTransit = SagaActorConstants.ExceptionOnTransit(Self.Path.Name);
            _sagaProducedCommand = SagaActorConstants.SagaProduceCommands(Self.Path.Name);

            var stateActorProps = Context.DI()
                                         .Props(typeof(SagaStateActor<TState>));

            _stateAggregateActor = Context.ActorOf(stateActorProps,
                                                   AggregateActorName.New<SagaStateAggregate<TState>>(Id)
                                                                     .Name);
            _stateAggregateActor.Tell(GetSagaState.Instance);

            Behavior.Become(InitializingBehavior, nameof(InitializingBehavior));
        }

        private void StashingMessagesToProcessBehavior()
        {
            Receive<IMessageMetadataEnvelop>(m => StashMessage(m));
            Receive<RedirectToNewSaga>(m => StashMessage(m));
            ProxifyingCommandsBehavior();
        }

        private void InitializingBehavior()
        {
            Receive<SagaState<TState>>(ss =>
                                       {
                                           if(ss.State != null ) //having some state already persisted
                                             Saga = _сreatorCatalog.Create(ss.State);
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

        private void StashMessage(object m)
        {
            _log.Warning("Saga {id} stashing message {messge}", Id, m);
            Stash.Stash();
        }

        private void AwaitingMessageBehavior()
        {
            Receive<IMessageMetadataEnvelop>(env =>
                                             {
                                                 if (_сreatorCatalog.CanCreateFrom(env.Message))
                                                 {
                                                     Self.Tell(new CreateNewSaga(env),Sender);
                                                     Behavior.Become(CreatingSagaBehavior, nameof(CreatingSagaBehavior));
                                                 }
                                                 else
                                                 {
                                                     Self.Tell(env, Sender);
                                                     Behavior.Become(TransitingSagaBehavior, nameof(TransitingSagaBehavior));
                                                 }
                                             });

            Receive<RedirectToNewSaga>(env =>
                                       {
                                           Self.Tell(new CreateNewSaga(env.MessageToRedirect, env.SagaId), Sender);
                                           Behavior.Become(CreatingSagaBehavior, nameof(CreatingSagaBehavior));
                                       });

            ProxifyingCommandsBehavior();
        }

        private class CreateNewSaga
        {
            public IMessageMetadataEnvelop Message { get; }
            public Guid? EnforcedId { get; }

            public CreateNewSaga(IMessageMetadataEnvelop message, Guid? enforcedId = null)
            {
                Message = message;
                EnforcedId = enforcedId;
            }
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

        private void CreatingSagaBehavior()
        {
            TState pendingState = null;
            IMessageMetadataEnvelop processingMessage = null;
            IActorRef processingMessageSender = null;
            Receive<CreateNewSaga>(c =>
                                   {
                                       processingMessage = c.Message;
                                       processingMessageSender = Sender;
                                       if (Saga != null)
                                           throw new SagaAlreadyStartedException(Saga.State, processingMessage);

                                       var saga = _сreatorCatalog.CreateNew(processingMessage.Message, c.EnforcedId);
                                       if (Id != saga.State.Id)
                                       {
                                           FinishSagaTransition(new RedirectToNewSaga(saga.State.Id, processingMessage), Sender);
                                           return;
                                       }
                                       pendingState = saga.State;
                                       var cmd = new CreateNewStateCommand<TState>(Id, pendingState);

                                       _stateAggregateActor.Ask<CommandCompleted>(new MessageMetadataEnvelop<ICommand>(cmd, processingMessage.Metadata))
                                                           .PipeTo(Self);
                                   });

            Receive<Status.Failure>(f => FinishWithError(processingMessage, f, processingMessageSender));

            //from state aggregate actro after persist
            Receive<CommandCompleted>(c =>
                                      {
                                          Saga = _сreatorCatalog.Create(pendingState);
                                          Self.Tell(processingMessage, processingMessageSender);
                                          Behavior.Become(TransitingSagaBehavior, nameof(TransitingSagaBehavior));
                                          pendingState = null;
                                          processingMessage = null;
                                          processingMessageSender = null;
                                      });
            StashingMessagesToProcessBehavior();

        }

        private void TransitingSagaBehavior()
        {
            IMessageMetadataEnvelop processingEnvelop = null;
            TState pendingState = null;
            IReadOnlyCollection<ICommand> producedCommands = new ICommand[]{};
            IActorRef processingMessageSender = null;


            Receive<IMessageMetadataEnvelop>(messageEnvelop =>
                                             {
                                                      processingEnvelop = messageEnvelop;
                                                      processingMessageSender = Sender;
                                                      if (Saga == null)
                                                      {
                                                          _log.Error("Saga {saga} {sagaid} is not started but received transition message {@message}. "
                                                                       + "Saga will not proceed. ", typeof(TState), Id, messageEnvelop);

                                                          Task.FromException(new SagaNotStartedException()).PipeTo(Self);
                                                          return;
                                                      }
                                                      if (Saga.State.Id != GetSagaId(messageEnvelop.Message))
                                                      {
                                                          _log.Error("Existing saga {saga} {sagaid} received message {@message} "
                                                                     + "targeting different saga. Saga will not proceed.", typeof(TState), Id, messageEnvelop);

                                                          Task.FromException(new SagaIdMismatchException()).PipeTo(Self);
                                                          return;
                                                      }

                                                      Task<TransitionResult<TState>> processSagaTask = Saga.PreviewTransit((dynamic)messageEnvelop.Message);
                                                      processSagaTask.PipeTo(Self);
                                                  });

            ReceiveAsync<TransitionResult<TState>>(transitionResult =>
                                              {
                                                  pendingState = transitionResult.State;
                                                  producedCommands = transitionResult.ProducedCommands;
                                                  var cmd = new SaveStateCommand<TState>(Id,
                                                                                         pendingState,
                                                                                         Saga.State.CurrentStateName,
                                                                                         processingEnvelop);

                                                  return _stateAggregateActor.Ask<CommandCompleted>(new MessageMetadataEnvelop<ICommand>(cmd, processingEnvelop.Metadata))
                                                                             .PipeTo(Self);
                                              });
            Receive<CommandCompleted>(c =>
                                      {
                                          Saga = _сreatorCatalog.Create(pendingState);
                                          FinishSagaTransition(new SagaTransited(producedCommands.ToArray(),
                                                                                 processingEnvelop.Metadata,
                                                                                 _sagaProducedCommand,
                                                                                 pendingState),
                                                               processingMessageSender);
                                          Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
                                          pendingState = null;
                                          processingMessageSender = null;
                                          processingEnvelop = null;
                                          producedCommands = null;
                                      });

            Receive<Status.Failure>(f => FinishWithError(processingEnvelop, f, processingMessageSender));

            StashingMessagesToProcessBehavior();
        }

        private void FinishWithError(IMessageMetadataEnvelop processingMessage, Status.Failure f, IActorRef messageSender)
        {
            var fault = CreateFault(processingMessage.Message,
                                    f.Cause.UnwrapSingle());

            var faultMetadata = processingMessage.Metadata.CreateChild(fault.SagaId, _exceptionOnTransit);

            _publisher.Publish(fault, faultMetadata);

            FinishSagaTransition(new SagaTransitFault(fault, processingMessage.Metadata), messageSender);
            
            Behavior.Become(AwaitingMessageBehavior, nameof(AwaitingMessageBehavior));
        }

        private void FinishSagaTransition(ISagaTransitCompleted message, IActorRef messageSender)
        {
            //notify saga process actor that saga transit is done
            foreach (var n in _sagaTransitionWaiters)
                n.Tell(message);

            messageSender.Tell(message);
            Stash.UnstashAll();
        }

        private IFault CreateFault(object message, Exception exception)
        {
            var processorType = Saga?.GetType() ?? typeof(TState);
            _log.Error(exception, "Saga {saga} {id} raised an error on {@message}", processorType, Id, message);
            return Fault.NewGeneric(message, exception, Id, processorType);
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